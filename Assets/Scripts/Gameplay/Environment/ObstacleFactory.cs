using UnityEngine;
using Zenject;

public class ObstacleFactory : IObstacleFactory
{
    private readonly DiContainer _container;
    private readonly ObstacleSpawnConfig _config;

    public ObstacleFactory(DiContainer container, ObstacleSpawnConfig config)
    {
        _container = container;
        _config = config;
    }

    public Obstacle Create(GameObject prefab, ObstacleType type, Vector3 position)
    {
        if (prefab == null)
        {
            Debug.LogError($"No {type} obstacle prefab assigned in ObstacleSpawnConfig.");
            return null;
        }

        GameObject instance = _container.InstantiatePrefab(prefab, position, Quaternion.identity, null);
        Obstacle obstacle = instance.GetComponent<Obstacle>();
        if (obstacle == null)
            obstacle = instance.AddComponent<BasicObstacle>();

        obstacle.SetType(type);
        return obstacle;
    }
}
