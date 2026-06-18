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

    public Obstacle Create(ObstacleType type, Vector3 position)
    {
        GameObject prefab = _config.GetPrefab(type);
        if (prefab == null)
        {
            Debug.LogError("No planet obstacle prefab assigned in ObstacleSpawnConfig.");
            return null;
        }

        return _container.InstantiatePrefabForComponent<Obstacle>(prefab, position, Quaternion.identity, null);
    }
}
