using System.Collections;
using UnityEngine;
using Zenject;

public class ObstacleSpawner : MonoBehaviour
{
    [Inject] private ObstaclePool _obstaclePool;
    [Inject] private ObstacleSpawnConfig _config;
    [Inject] private ObstacleManipulator _obstacleMover;
    [Inject] private SpeedManager _speedManager;
    [Inject] private GameConfig _gameConfig;

    private float _nextSpawnTime;
    private float _currentSpawnInterval;

    private void Start()
    {
        enabled = false;
        _nextSpawnTime = Time.time + _config.initialSafeZone;
    }

    private void Update()
    {
        if (!enabled) return;

        float speedFactor = _speedManager.CurrentSpeed / _gameConfig.initialSpeed;

        _currentSpawnInterval = Mathf.Max(_config.baseSpawnInterval / speedFactor, _config.minSpawnInterval);

        Debug.Log(_gameConfig.initialSpeed);
        Debug.Log(_speedManager.CurrentSpeed);

        Debug.Log($"[Spawner] Time: {Time.time}, nextSpawn: {_nextSpawnTime}, interval: {_currentSpawnInterval}, speedFactor: {speedFactor}");

        if (Time.time >= _nextSpawnTime)
        {
            SpawnObstacle();
            //_nextSpawnTime = Time.time + _currentSpawnInterval;
        }
    }

    private void SpawnObstacle()
    {
        ObstacleType type = _config.GetRandomType();
        float x = Random.Range(-1, 2) * _config.laneWidth;
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
        _nextSpawnTime = Time.time + _config.initialSafeZone;
    }
}