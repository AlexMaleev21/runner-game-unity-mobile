using UnityEngine;

public class JumpingState : IPlayerState
{
    private readonly PlayerController _player;
    private readonly PlayerConfig _config;
    private Rigidbody _rigidbody;
    private float _jumpTime;
    public PlayerStateType StateType => PlayerStateType.Jumping;

    public JumpingState(PlayerController player, PlayerConfig config)
    {
        _player = player;
        _config = config;
        _rigidbody = player.GetComponentInChildren<Rigidbody>();
    }

    public void Enter()
    {
        _player.SetAnimationTrigger("Jump");
        _rigidbody.AddForce(Vector3.up * _config.jumpForce, ForceMode.Impulse);
        _jumpTime = Time.time;
    }

    public void Update()
    {
        if (Time.time > _jumpTime + _config.jumpDuration)
        {
            _player.StateMachine.ChangeState(PlayerStateType.Running);
        }
    }

    public void Exit()
    {

    }
}
