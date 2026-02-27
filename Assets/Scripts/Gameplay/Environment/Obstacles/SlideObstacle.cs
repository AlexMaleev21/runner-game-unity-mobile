using UnityEngine;

public class SlideObstacle : Obstacle
{

    private void Start()
    {
        _type = ObstacleType.SlideObstacle;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player != null && player.StateMachine.CurrentState.StateType != PlayerStateType.Sliding)
        {
            player.Die();
        }
    }
}
