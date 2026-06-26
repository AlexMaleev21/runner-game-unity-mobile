using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObstaclePool
{
    private readonly Dictionary<GameObject, Queue<Obstacle>> _pools = new Dictionary<GameObject, Queue<Obstacle>>();
    private readonly Dictionary<Obstacle, GameObject> _prefabsByObstacle = new Dictionary<Obstacle, GameObject>();
    private readonly IObstacleFactory _factory;
    private readonly ObstacleSpawnConfig _config;
    private readonly Transform _poolParent;

    public ObstaclePool(IObstacleFactory factory, ObstacleSpawnConfig config)
    {
        _factory = factory;
        _config = config;

        _poolParent = new GameObject("ObstaclePool").transform;
        Prewarm(_config.AsteroidPrefabs, ObstacleType.Asteroid);
        Prewarm(_config.PlanetPrefabs, ObstacleType.Planet);

        if (_pools.Count == 0 && _config.ObstaclePrefab != null)
            Prewarm(new[] { _config.ObstaclePrefab }, ObstacleType.Asteroid);
    }

    public Obstacle Get(GameObject prefab, ObstacleType type, Vector3 position)
    {
        if (prefab == null)
            return null;

        if (!_pools.TryGetValue(prefab, out Queue<Obstacle> pool))
        {
            pool = new Queue<Obstacle>();
            _pools[prefab] = pool;
        }

        if (pool.Count > 0)
        {
            var obstacle = pool.Dequeue();
            obstacle.transform.position = position;
            obstacle.SetType(type);
            obstacle.OnSpawn();
            return obstacle;
        }

        return CreatePooledObstacle(prefab, type, position, true);
    }

    public void Return(Obstacle obstacle)
    {
        if (obstacle == null)
            return;

        obstacle.OnDespawn();
        obstacle.transform.SetParent(_poolParent);
        if (!_prefabsByObstacle.TryGetValue(obstacle, out GameObject prefab) || prefab == null)
            return;

        if (!_pools.TryGetValue(prefab, out Queue<Obstacle> pool))
        {
            pool = new Queue<Obstacle>();
            _pools[prefab] = pool;
        }

        pool.Enqueue(obstacle);
    }

    private void Prewarm(GameObject[] prefabs, ObstacleType type)
    {
        if (prefabs == null || prefabs.Length == 0)
            return;

        int perPrefabPoolSize = Mathf.Max(1, Mathf.CeilToInt((float)_config.ObstaclePoolSize / prefabs.Length));
        for (int i = 0; i < prefabs.Length; i++)
        {
            GameObject prefab = prefabs[i];
            if (prefab == null)
                continue;

            if (!_pools.ContainsKey(prefab))
                _pools[prefab] = new Queue<Obstacle>();

            for (int j = 0; j < perPrefabPoolSize; j++)
                CreatePooledObstacle(prefab, type, Vector3.zero, false);
        }
    }

    private Obstacle CreatePooledObstacle(GameObject prefab, ObstacleType type, Vector3 position, bool spawn)
    {
        var obstacle = _factory.Create(prefab, type, position);
        if (obstacle == null)
            return null;

        obstacle.transform.SetParent(_poolParent);
        _prefabsByObstacle[obstacle] = prefab;

        if (spawn)
        {
            obstacle.OnSpawn();
        }
        else
        {
            obstacle.OnDespawn();
            _pools[prefab].Enqueue(obstacle);
        }

        return obstacle;
    }
}
