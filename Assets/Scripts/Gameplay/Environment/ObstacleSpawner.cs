using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObstacleSpawner : MonoBehaviour
{
    private readonly List<int> _laneBuffer = new List<int> { -1, 0, 1 };

    private ObstaclePool _obstaclePool;
    private ObstacleSpawnConfig _config;
    private ObstacleManipulator _obstacleMover;
    private PlayerConfig _playerConfig;

    private float _nextSpawnTime;
    private float _currentSpawnInterval;

    [Inject]
    public void Construct(
        ObstaclePool obstaclePool,
        ObstacleSpawnConfig obstacleSpawnConfig,
        ObstacleManipulator obstacleManipulator,
        PlayerConfig playerConfig)
    {
        _obstaclePool = obstaclePool;
        _config = obstacleSpawnConfig;
        _obstacleMover = obstacleManipulator;
        _playerConfig = playerConfig;
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
        _currentSpawnInterval = Mathf.Max(_config.BaseSpawnInterval, _config.MinSpawnInterval, safeJumpInterval);

        if (Time.time >= _nextSpawnTime)
        {
            SpawnObstacleRow();
        }
    }

    private void SpawnObstacleRow()
    {
        int obstacleCount = Random.Range(_config.MinObstaclesPerRow, _config.MaxObstaclesPerRow + 1);
        ShuffleLanes();

        for (int i = 0; i < obstacleCount; i++)
        {
            int lane = _laneBuffer[i];
            Vector3 spawnPos = new Vector3(lane * _config.LaneWidth, _config.ObstacleY, _config.SpawnZ);
            Obstacle obstacle = _obstaclePool.Get(_config.ObstacleType, spawnPos);

            if (obstacle != null)
                _obstacleMover.RegisterObstacle(obstacle);
        }

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
