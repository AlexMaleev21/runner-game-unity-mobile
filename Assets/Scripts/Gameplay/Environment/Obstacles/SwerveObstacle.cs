using UnityEngine;

public class SwerveObstacle : Obstacle
{

    private void Awake()
    {
        _type = ObstacleType.Planet;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.Die();
    }
}
