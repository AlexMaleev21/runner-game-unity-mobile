using UnityEngine;

public class BasicObstacle : Obstacle
{
    protected override void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null)
            player.Die();
    }
}
