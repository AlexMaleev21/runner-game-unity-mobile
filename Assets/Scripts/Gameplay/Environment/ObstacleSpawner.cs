using System.Collections;
using UnityEngine;
using Zenject;

public class ObstacleSpawner : MonoBehaviour
{
    private ObstaclePool _obstaclePool;
    private ObstacleSpawnConfig _config;
    private ObstacleManipulator _obstacleMover;
    private SpeedManager _speedManager;
    private GameConfig _gameConfig;

    private float _nextSpawnTime;
    private float _currentSpawnInterval;

    [Inject]
    public void Construct(
        ObstaclePool obstaclePool,
        ObstacleSpawnConfig obstacleSpawnConfig,
        ObstacleManipulator obstacleManipulator,
        SpeedManager speedManager,
        GameConfig gameConfig)
    {
        _obstaclePool = obstaclePool;
        _config = obstacleSpawnConfig;
        _obstacleMover = obstacleManipulator;
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

        float speedFactor = _speedManager.CurrentSpeed / _gameConfig.InitialSpeed;

        _currentSpawnInterval = Mathf.Max(_config.BaseSpawnInterval / speedFactor, _config.MinSpawnInterval);

        if (Time.time >= _nextSpawnTime)
        {
            SpawnObstacle();
            //_nextSpawnTime = Time.time + _currentSpawnInterval;
        }
    }

    private void SpawnObstacle()
    {
        ObstacleType type = _config.GetRandomType();
        float x = Random.Range(-1, 2) * _config.LaneWidth;
        Vector3 spawnPos = new Vector3(x, 0, 20f);

        Obstacle obstacle = _obstaclePool.Get(type, spawnPos);
        _obstacleMover.RegisterObstacle(obstacle);

        _nextSpawnTime = Time.time + _currentSpawnInterval;
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
    }
}