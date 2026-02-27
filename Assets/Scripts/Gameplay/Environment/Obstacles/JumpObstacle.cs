using UnityEngine;

public class JumpObstacle : Obstacle
{
    private void Start()
    {
        _type = ObstacleType.JumpObstacle;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponentInParent<PlayerController>();
        Debug.Log(player);
        if (player != null && player.StateMachine.CurrentState.StateType != PlayerStateType.Jumping)
        {
            player.Die();
        }
    }
}
