using UnityEngine;
using Zenject;

public class CoinSpawner : ITickable
{
    private readonly CoinSpawnConfig _config;
    private readonly CoinPool _coinPool;
    private readonly CoinMover _coinMover;
    private readonly SpeedManager _speedManager;
    private readonly LayerMask _obstacleLayer;
    private float _nextSpawnTime;
    private bool _isActive = false;
    private float _minDistanceBetweenCoins = 2.5f;
    private float _spawnZBase = 20f;

    public CoinSpawner(CoinSpawnConfig config, CoinPool coinPool, CoinMover coinMover, SpeedManager speedManager)
    {
        _config = config;
        _coinPool = coinPool;
        _coinMover = coinMover;
        _speedManager = speedManager;
        _obstacleLayer = LayerMask.GetMask("Obstacle");
    }

    public void StartSpawning() { _isActive = true; _nextSpawnTime = Time.time + 1f; }
    public void StopSpawning() { _isActive = false; }

    public void Tick()
    {
        if (!_isActive) return;
        if (Time.time < _nextSpawnTime) return;

        float speed = _speedManager.CurrentSpeed;
        float speedFactor = Mathf.Max(speed / 5f, 0.5f);
        float interval = Mathf.Max(_config.spawnInterval / speedFactor, _config.minSpawnInterval);
        _nextSpawnTime = Time.time + interval;

        SpawnCoinsBatch();
    }

    private void SpawnCoinsBatch()
    {
        int coinsToSpawn = Random.Range(1, _config.maxCoinsPerSpawn + 1);
        float zOffset = 0f;

        for (int i = 0; i < coinsToSpawn; i++)
        {
            float x = Random.Range(-_config.laneWidth, _config.laneWidth);
            float z = _spawnZBase + zOffset;
            Vector3 pos = new Vector3(x, 0, z);

            Collider[] hitColliders = Physics.OverlapBox(pos, new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, _obstacleLayer);
            if (hitColliders.Length > 0)
                continue;

            var coin = _coinPool.Get(pos);
            Debug.Log($"Spawned coin at X={pos.x}, Z={pos.z}");
            _coinMover.RegisterCoin(coin);
            zOffset += _minDistanceBetweenCoins;
        }
    }
}