using UnityEngine;

public interface IObstacleFactory
{
    Obstacle Create(GameObject prefab, ObstacleType type, Vector3 position);
}
