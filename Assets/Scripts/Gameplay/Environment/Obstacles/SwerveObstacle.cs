using UnityEngine;

public class SwerveObstacle : Obstacle
{

    private void Start()
    {
        _type = ObstacleType.SwerveObstacle;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        player.Die();
    }
}
