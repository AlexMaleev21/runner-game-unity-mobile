using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner
{
    private enum CoinSnakePattern
    {
        Straight,
        ZigZag,
        DriftLeft,
        DriftRight,
        Wave,
        Arc
    }

    private readonly List<int> _sideLaneBuffer = new List<int>(2);

    private readonly CoinSpawnConfig _coinConfig;
    private readonly ObstacleSpawnConfig _obstacleConfig;
    private readonly CoinPool _coinPool;
    private readonly CoinManipulator _coinManipulator;
    private readonly SpeedManager _speedManager;
    private readonly GameConfig _gameConfig;

    public CoinSpawner(
        CoinSpawnConfig coinConfig,
        ObstacleSpawnConfig obstacleConfig,
        CoinPool coinPool,
        CoinManipulator coinManipulator,
        SpeedManager speedManager,
        GameConfig gameConfig)
    {
        _coinConfig = coinConfig;
        _obstacleConfig = obstacleConfig;
        _coinPool = coinPool;
        _coinManipulator = coinManipulator;
        _speedManager = speedManager;
        _gameConfig = gameConfig;
    }

    public void SpawnCoinRow(IReadOnlyCollection<int> occupiedObstacleLanes, float obstacleRowZ)
    {
        if (Random.value > _coinConfig.RowSpawnChance)
            return;

        int coinCount = Random.Range(_coinConfig.MinCoinsPerRow, _coinConfig.MaxCoinsPerRow + 1);
        bool shouldSpawnArcOverObstacle = occupiedObstacleLanes != null
            && occupiedObstacleLanes.Count > 0
            && (occupiedObstacleLanes.Count >= 3 || Random.value < _coinConfig.ArcOverObstacleChance);

        if (shouldSpawnArcOverObstacle)
        {
            SpawnArcOverObstacleRow(occupiedObstacleLanes, obstacleRowZ, coinCount);
            return;
        }

        SpawnFreeFormRow(occupiedObstacleLanes, obstacleRowZ, coinCount);
    }

    private void SpawnArcOverObstacleRow(IReadOnlyCollection<int> occupiedObstacleLanes, float obstacleRowZ, int coinCount)
    {
        int centerIndex = coinCount / 2;
        int obstacleLane = GetRandomOccupiedLane(occupiedObstacleLanes);
        float spacing = GetAboveObstacleCoinSpacing();
        float startZ = obstacleRowZ - centerIndex * spacing;

        for (int i = 0; i < coinCount; i++)
        {
            float coinZ = startZ + i * spacing;
            float coinY = GetObstacleArcY(i, centerIndex, coinCount);
            Vector3 position = new Vector3(obstacleLane * _obstacleConfig.LaneWidth, coinY, coinZ);
            Coin coin = _coinPool.Get(position);
            _coinManipulator.RegisterCoin(coin);
        }
    }

    private void SpawnFreeFormRow(IReadOnlyCollection<int> occupiedObstacleLanes, float obstacleRowZ, int coinCount)
    {
        CoinSnakePattern pattern = (CoinSnakePattern)Random.Range(0, System.Enum.GetValues(typeof(CoinSnakePattern)).Length);
        int currentLane = Random.Range(-1, 2);
        int startLane = currentLane;
        float currentY = _coinConfig.CoinY;
        float coinZ = obstacleRowZ;
        bool hasCoinAboveObstacle = false;

        for (int i = 0; i < coinCount; i++)
        {
            if (i > 0)
                coinZ += _coinConfig.CoinSpacingZ;

            if (i > 0)
                currentLane = GetNextLane(pattern, startLane, currentLane, i);

            currentY = GetPatternY(pattern, i, coinCount, currentY);
            CoinObstacleAdjustment adjustment = ResolveObstacleOverlap(
                occupiedObstacleLanes,
                obstacleRowZ,
                coinZ,
                hasCoinAboveObstacle,
                ref currentLane,
                ref currentY);

            hasCoinAboveObstacle = hasCoinAboveObstacle || adjustment == CoinObstacleAdjustment.Above;

            Vector3 position = new Vector3(currentLane * _obstacleConfig.LaneWidth, currentY, coinZ);
            Coin coin = _coinPool.Get(position);
            _coinManipulator.RegisterCoin(coin);
        }
    }

    private int GetNextLane(CoinSnakePattern pattern, int startLane, int currentLane, int index)
    {
        switch (pattern)
        {
            case CoinSnakePattern.ZigZag:
                return ClampLane(currentLane + (index % 2 == 0 ? -1 : 1));

            case CoinSnakePattern.DriftLeft:
                return ClampLane(currentLane - (index % 2));

            case CoinSnakePattern.DriftRight:
                return ClampLane(currentLane + (index % 2));

            case CoinSnakePattern.Wave:
                return ClampLane(startLane + GetWaveOffset(index));

            default:
                return currentLane;
        }
    }

    private float GetPatternY(CoinSnakePattern pattern, int index, int count, float currentY)
    {
        if (pattern != CoinSnakePattern.Arc || count <= 1)
            return currentY;

        float progress = index / (float)(count - 1);
        return _coinConfig.CoinY + Mathf.Sin(progress * Mathf.PI) * (_coinConfig.AboveObstacleY - _coinConfig.CoinY);
    }

    private CoinObstacleAdjustment ResolveObstacleOverlap(
        IReadOnlyCollection<int> occupiedObstacleLanes,
        float obstacleRowZ,
        float coinZ,
        bool hasCoinAboveObstacle,
        ref int lane,
        ref float y)
    {
        if (occupiedObstacleLanes == null || !ContainsLane(occupiedObstacleLanes, lane))
            return CoinObstacleAdjustment.None;

        if (Mathf.Abs(coinZ - obstacleRowZ) > _coinConfig.ObstacleZClearance)
            return CoinObstacleAdjustment.None;

        bool preferSide = hasCoinAboveObstacle || Random.value < 0.5f;
        if (preferSide && TryMoveSide(occupiedObstacleLanes, lane, out int sideLane))
        {
            lane = sideLane;
            y = _coinConfig.CoinY;
            return CoinObstacleAdjustment.Side;
        }

        if (hasCoinAboveObstacle)
            return CoinObstacleAdjustment.None;

        y = _coinConfig.AboveObstacleY;
        return CoinObstacleAdjustment.Above;
    }

    private float GetAboveObstacleCoinSpacing()
    {
        float initialSpeed = Mathf.Max(_gameConfig.InitialSpeed, 0.01f);
        float speedFactor = Mathf.Max(_speedManager.CurrentSpeed / initialSpeed, 1f);
        return _coinConfig.AboveObstacleCoinSpacingZ * speedFactor;
    }

    private float GetObstacleArcY(int index, int centerIndex, int count)
    {
        if (count <= 1)
            return _coinConfig.AboveObstacleY;

        float progress = index / (float)(count - 1);
        return _coinConfig.CoinY + Mathf.Sin(progress * Mathf.PI) * (_coinConfig.AboveObstacleY - _coinConfig.CoinY);
    }

    private int GetRandomOccupiedLane(IReadOnlyCollection<int> occupiedObstacleLanes)
    {
        int targetIndex = Random.Range(0, occupiedObstacleLanes.Count);
        int index = 0;

        foreach (int lane in occupiedObstacleLanes)
        {
            if (index == targetIndex)
                return lane;

            index++;
        }

        return 0;
    }

    private bool TryMoveSide(IReadOnlyCollection<int> occupiedObstacleLanes, int lane, out int sideLane)
    {
        _sideLaneBuffer.Clear();

        int left = lane - 1;
        int right = lane + 1;

        if (IsLaneAvailable(left, occupiedObstacleLanes))
            _sideLaneBuffer.Add(left);

        if (IsLaneAvailable(right, occupiedObstacleLanes))
            _sideLaneBuffer.Add(right);

        if (_sideLaneBuffer.Count == 0)
        {
            sideLane = lane;
            return false;
        }

        sideLane = _sideLaneBuffer[Random.Range(0, _sideLaneBuffer.Count)];
        return true;
    }

    private bool IsLaneAvailable(int lane, IReadOnlyCollection<int> occupiedObstacleLanes)
    {
        return lane >= -1 && lane <= 1 && !ContainsLane(occupiedObstacleLanes, lane);
    }

    private bool ContainsLane(IReadOnlyCollection<int> lanes, int lane)
    {
        foreach (int occupiedLane in lanes)
        {
            if (occupiedLane == lane)
                return true;
        }

        return false;
    }

    private int GetWaveOffset(int index)
    {
        int phase = index % 4;
        if (phase == 1)
            return 1;

        if (phase == 3)
            return -1;

        return 0;
    }

    private int ClampLane(int lane)
    {
        return Mathf.Clamp(lane, -1, 1);
    }

    private enum CoinObstacleAdjustment
    {
        None,
        Side,
        Above
    }
}
