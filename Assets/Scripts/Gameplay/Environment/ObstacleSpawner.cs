using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObstacleSpawner : MonoBehaviour
{
    private readonly List<int> _laneBuffer = new List<int> { -1, 0, 1 };
    private readonly List<int> _occupiedObstacleLanes = new List<int>(3);

    private ObstaclePool _obstaclePool;
    private ObstacleSpawnConfig _config;
    private ObstacleManipulator _obstacleMover;
    private CoinSpawner _coinSpawner;
    private PlayerConfig _playerConfig;
    private SpeedManager _speedManager;
    private GameConfig _gameConfig;

    private float _nextSpawnTime;
    private float _currentSpawnInterval;
    private int _previousObstacleCount;

    [Inject]
    public void Construct(
        ObstaclePool obstaclePool,
        ObstacleSpawnConfig obstacleSpawnConfig,
        ObstacleManipulator obstacleManipulator,
        CoinSpawner coinSpawner,
        PlayerConfig playerConfig,
        SpeedManager speedManager,
        GameConfig gameConfig)
    {
        _obstaclePool = obstaclePool;
        _config = obstacleSpawnConfig;
        _obstacleMover = obstacleManipulator;
        _coinSpawner = coinSpawner;
        _playerConfig = playerConfig;
        _speedManager = speedManager;
        _gameConfig = gameConfig;
    }
    private void Start()
    {
        enabled = false;
        _nextSpawnTime = Time.time + _config.InitialSafeZone;
    }

    private void Update()
    {
        if (!enabled) return;

        float safeJumpInterval = _playerConfig.JumpDuration + _config.PostJumpSafetyTime;
        float speedProgress = GetSpeedProgress();
        float intervalReduction = _config.MaxSpeedIntervalReduction * speedProgress;
        float speedScaledInterval = _config.BaseSpawnInterval * (1f - intervalReduction);

        _currentSpawnInterval = Mathf.Max(_config.MinSpawnInterval, safeJumpInterval, speedScaledInterval);

        if (Time.time >= _nextSpawnTime)
        {
            SpawnObstacleRow();
        }
    }

    private void SpawnObstacleRow()
    {
        int minObstacleCount = _previousObstacleCount >= 3
            ? Mathf.Min(_config.MinObstaclesPerRow, 2)
            : _config.MinObstaclesPerRow;
        int maxObstacleCount = _previousObstacleCount >= 3
            ? Mathf.Min(_config.MaxObstaclesPerRow, 2)
            : _config.MaxObstaclesPerRow;

        int obstacleCount = Random.Range(minObstacleCount, maxObstacleCount + 1);
        ShuffleLanes();
        _occupiedObstacleLanes.Clear();

        for (int i = 0; i < obstacleCount; i++)
        {
            int lane = _laneBuffer[i];
            _occupiedObstacleLanes.Add(lane);

            Vector3 spawnPos = new Vector3(lane * _config.LaneWidth, _config.ObstacleY, _config.SpawnZ);
            Obstacle obstacle = _obstaclePool.Get(_config.ObstacleType, spawnPos);

            if (obstacle != null)
                _obstacleMover.RegisterObstacle(obstacle);
        }

        _coinSpawner.SpawnCoinRow(_occupiedObstacleLanes, _config.SpawnZ);
        _previousObstacleCount = obstacleCount;
        _nextSpawnTime = Time.time + _currentSpawnInterval;
    }

    private void ShuffleLanes()
    {
        for (int i = 0; i < _laneBuffer.Count; i++)
        {
            int randomIndex = Random.Range(i, _laneBuffer.Count);
            (_laneBuffer[i], _laneBuffer[randomIndex]) = (_laneBuffer[randomIndex], _laneBuffer[i]);
        }
    }

    private float GetSpeedProgress()
    {
        float initialSpeed = Mathf.Max(_gameConfig.InitialSpeed, 0.01f);
        float maxSpeed = Mathf.Max(_gameConfig.MaxSpeed, initialSpeed);

        return Mathf.InverseLerp(initialSpeed, maxSpeed, _speedManager.CurrentSpeed);
    }

    public void PauseSpawn(float duration)
    {
        StartCoroutine(PauseSpawnCoroutine(duration));
    }

    private IEnumerator PauseSpawnCoroutine(float duration)
    {
        enabled = false;
        yield return new WaitForSeconds(duration);
        enabled = true;
    }

    public void ResetSpawner()
    {
        _nextSpawnTime = Time.time + _config.InitialSafeZone;
        _previousObstacleCount = 0;
    }
}
