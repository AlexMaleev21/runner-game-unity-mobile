using UnityEngine;

public interface IObstacleFactory
{
    Obstacle Create(ObstacleType type, Vector3 position);
}
