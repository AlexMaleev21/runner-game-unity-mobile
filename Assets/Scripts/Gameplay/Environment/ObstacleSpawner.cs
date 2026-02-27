using System.Collections;
using UnityEngine;
using Zenject;

public class ObstacleSpawner : MonoBehaviour
{
    [Inject] private ObstaclePool _obstaclePool;
    [Inject] private ObstacleSpawnConfig _config;
    [Inject] private ObstacleMover _obstacleMover;

    private float _nextSpawnTime;

    private void Start()
    {
        enabled = false;
        _nextSpawnTime = Time.time + _config.initialSafeZone;
    }

    private void Update()
    {
        if (!enabled) return;

        if (Time.time >= _nextSpawnTime)
        {
            SpawnObstacle();
        }
    }

    private void SpawnObstacle()
    {
        ObstacleType type = _config.GetRandomType();
        float x = Random.Range(-1, 2) * _config.laneWidth;
        Vector3 spawnPos = new Vector3(x, 0, 20f);

        Obstacle obstacle = _obstaclePool.Get(type, spawnPos);
        _obstacleMover.RegisterObstacle(obstacle);

        _nextSpawnTime = Time.time + _config.spawnInterval;
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