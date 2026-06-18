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
        Wave
    }

    private readonly List<int> _sideLaneBuffer = new List<int>(2);
    private readonly CoinSpawnConfig _coinConfig;
    private readonly ObstacleSpawnConfig _obstacleConfig;
    private readonly CoinPool _coinPool;
    private readonly CoinManipulator _coinManipulator;
    private readonly SpeedManager _speedManager;
    private readonly GameConfig _gameConfig;
    private readonly PlayerConfig _playerConfig;

    public CoinSpawner(
        CoinSpawnConfig coinConfig,
        ObstacleSpawnConfig obstacleConfig,
        CoinPool coinPool,
        CoinManipulator coinManipulator,
        SpeedManager speedManager,
        GameConfig gameConfig,
        PlayerConfig playerConfig)
    {
        _coinConfig = coinConfig;
        _obstacleConfig = obstacleConfig;
        _coinPool = coinPool;
        _coinManipulator = coinManipulator;
        _speedManager = speedManager;
        _gameConfig = gameConfig;
        _playerConfig = playerConfig;
    }

    public void SpawnCoinRow(IReadOnlyCollection<int> occupiedObstacleLanes, float obstacleRowZ)
    {
        if (Random.value > _coinConfig.RowSpawnChance)
            return;

        int coinCount = Random.Range(_coinConfig.MinCoinsPerRow, _coinConfig.MaxCoinsPerRow + 1);
        SpawnFreeFormRow(occupiedObstacleLanes, obstacleRowZ, coinCount);
    }

    private void SpawnFreeFormRow(IReadOnlyCollection<int> occupiedObstacleLanes, float obstacleRowZ, int coinCount)
    {
        int currentLane = Random.Range(-1, 2);
        bool startsOnObstacle = occupiedObstacleLanes != null && ContainsLane(occupiedObstacleLanes, currentLane);
        bool useObstacleLane = false;

        if (startsOnObstacle)
        {
            useObstacleLane = occupiedObstacleLanes.Count >= 3 || Random.value < _coinConfig.ArcOverObstacleChance;
            if (!useObstacleLane && TryMoveSide(occupiedObstacleLanes, currentLane, out int sideLane))
                currentLane = sideLane;
            else
                useObstacleLane = true;
        }

        CoinSnakePattern pattern = useObstacleLane ? CoinSnakePattern.Straight : GetRandomFreeFormPattern();
        int startLane = currentLane;

        if (!useObstacleLane
            && TryGetFirstObstacleOverlapLane(occupiedObstacleLanes, obstacleRowZ, startLane, pattern, coinCount, out int obstacleLane))
        {
            useObstacleLane = true;
            pattern = CoinSnakePattern.Straight;
            currentLane = obstacleLane;
            startLane = obstacleLane;
        }

        float currentY = _coinConfig.CoinY;
        float coinZ = obstacleRowZ;

        for (int i = 0; i < coinCount; i++)
        {
            if (i > 0)
                coinZ += useObstacleLane ? GetAboveObstacleCoinSpacing() : GetCoinSpacing(_coinConfig.CoinSpacingZ);

            if (i > 0)
                currentLane = GetNextLane(pattern, startLane, currentLane, i);

            currentY = _coinConfig.CoinY;
            ResolveObstacleOverlap(
                occupiedObstacleLanes,
                obstacleRowZ,
                coinZ,
                useObstacleLane,
                ref currentLane,
                ref currentY);

            Vector3 position = new Vector3(currentLane * _obstacleConfig.LaneWidth, currentY, coinZ);
            Coin coin = _coinPool.Get(position);
            _coinManipulator.RegisterCoin(coin);
        }
    }

    private CoinSnakePattern GetRandomFreeFormPattern()
    {
        return (CoinSnakePattern)Random.Range(0, System.Enum.GetValues(typeof(CoinSnakePattern)).Length);
    }

    private bool TryGetFirstObstacleOverlapLane(
        IReadOnlyCollection<int> occupiedObstacleLanes,
        float obstacleRowZ,
        int startLane,
        CoinSnakePattern pattern,
        int coinCount,
        out int obstacleLane)
    {
        obstacleLane = startLane;

        if (occupiedObstacleLanes == null || occupiedObstacleLanes.Count == 0)
            return false;

        int previewLane = startLane;
        float previewZ = obstacleRowZ;

        for (int i = 0; i < coinCount; i++)
        {
            if (i > 0)
            {
                previewZ += GetCoinSpacing(_coinConfig.CoinSpacingZ);
                previewLane = GetNextLane(pattern, startLane, previewLane, i);
            }

            if (Mathf.Abs(previewZ - obstacleRowZ) <= _coinConfig.ObstacleZClearance
                && ContainsLane(occupiedObstacleLanes, previewLane))
            {
                obstacleLane = previewLane;
                return true;
            }
        }

        return false;
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

    private void ResolveObstacleOverlap(
        IReadOnlyCollection<int> occupiedObstacleLanes,
        float obstacleRowZ,
        float coinZ,
        bool allowAboveObstacle,
        ref int lane,
        ref float y)
    {
        if (occupiedObstacleLanes == null || !ContainsLane(occupiedObstacleLanes, lane))
            return;

        if (Mathf.Abs(coinZ - obstacleRowZ) > _coinConfig.ObstacleZClearance)
            return;

        if (!allowAboveObstacle && TryMoveSide(occupiedObstacleLanes, lane, out int sideLane))
        {
            lane = sideLane;
            y = _coinConfig.CoinY;
            return;
        }

        y = GetAboveObstacleY();
    }

    private float GetAboveObstacleCoinSpacing()
    {
        float spacing = GetCoinSpacing(_coinConfig.AboveObstacleCoinSpacingZ);
        return Mathf.Max(spacing, _coinConfig.ObstacleZClearance + 0.05f);
    }

    private float GetCoinSpacing(float baseSpacing)
    {
        float speedProgress = GetSpeedProgress();
        float spacingMultiplier = 1f + speedProgress * _coinConfig.CoinSpacingSpeedScale;
        spacingMultiplier = Mathf.Min(spacingMultiplier, _coinConfig.MaxCoinSpacingSpeedMultiplier);

        return baseSpacing * spacingMultiplier;
    }

    private float GetSpeedProgress()
    {
        float initialSpeed = Mathf.Max(_gameConfig.InitialSpeed, 0.01f);
        float maxSpeed = Mathf.Max(_gameConfig.MaxSpeed, initialSpeed);

        return Mathf.InverseLerp(initialSpeed, maxSpeed, _speedManager.CurrentSpeed);
    }

    private float GetAboveObstacleY()
    {
        float configuredJumpHeight = Mathf.Max(0f, _playerConfig.JumpHeight);
        float forceBasedHeight = 0f;

        if (_playerConfig.JumpForce > 0f && Mathf.Abs(Physics.gravity.y) > 0.01f)
        {
            forceBasedHeight = (_playerConfig.JumpForce * _playerConfig.JumpForce) / (2f * Mathf.Abs(Physics.gravity.y));
        }

        float jumpHeight = configuredJumpHeight > 0f ? configuredJumpHeight : forceBasedHeight;

        if (jumpHeight <= 0f)
            return _coinConfig.AboveObstacleY;

        return _coinConfig.CoinY + jumpHeight * _coinConfig.AboveObstacleJumpHeightMultiplier;
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

}
