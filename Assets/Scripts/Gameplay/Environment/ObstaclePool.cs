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

        foreach (var typeData in _config.obstacleTypes)
        {
            _pools[typeData.type] = new Queue<Obstacle>();

            for (int i = 0; i < _config.ObstaclePoolSize; i++)
            {
                var obstacle = _factory.Create(typeData.type, Vector3.zero);
                obstacle.transform.SetParent(_poolParent);
                obstacle.OnDespawn();
                _pools[typeData.type].Enqueue(obstacle);
            }
        }
    }

    public Obstacle Get(ObstacleType type, Vector3 position)
    {
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
            obstacle.transform.SetParent(_poolParent);
            obstacle.OnSpawn();
            return obstacle;
        }
    }

    public void Return(Obstacle obstacle)
    {
        obstacle.OnDespawn();
        obstacle.transform.SetParent(_poolParent);
        _pools[obstacle.Type].Enqueue(obstacle);
    }
}
