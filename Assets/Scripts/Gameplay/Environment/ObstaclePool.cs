using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObstaclePool
{
    private readonly Dictionary<ObstacleType, Queue<Obstacle>> _pools = new Dictionary<ObstacleType, Queue<Obstacle>>();
    private readonly IObstacleFactory _factory;
    private readonly ObstacleSpawnConfig _config;
    private readonly Transform _poolParent;

    public ObstaclePool(IObstacleFactory factory, ObstacleSpawnConfig config)
    {
        _factory = factory;
        _config = config;

        _poolParent = new GameObject("ObstaclePool").transform;
        _pools[_config.ObstacleType] = new Queue<Obstacle>();

        for (int i = 0; i < _config.ObstaclePoolSize; i++)
        {
            var obstacle = _factory.Create(_config.ObstacleType, Vector3.zero);
            if (obstacle == null)
                continue;

            obstacle.transform.SetParent(_poolParent);
            obstacle.OnDespawn();
            _pools[_config.ObstacleType].Enqueue(obstacle);
        }
    }

    public Obstacle Get(ObstacleType type, Vector3 position)
    {
        type = _config.ObstacleType;

        if (_pools[type].Count > 0)
        {
            var obstacle = _pools[type].Dequeue();
            obstacle.transform.position = position;
            obstacle.OnSpawn();
            return obstacle;
        }
        else
        {
            var obstacle = _factory.Create(type, position);
            if (obstacle == null)
                return null;

            obstacle.transform.SetParent(_poolParent);
            obstacle.OnSpawn();
            return obstacle;
        }
    }

    public void Return(Obstacle obstacle)
    {
        if (obstacle == null)
            return;

        obstacle.OnDespawn();
        obstacle.transform.SetParent(_poolParent);
        _pools[_config.ObstacleType].Enqueue(obstacle);
    }
}
