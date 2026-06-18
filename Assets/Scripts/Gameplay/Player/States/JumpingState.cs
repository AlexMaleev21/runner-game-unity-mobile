using UnityEngine;

public class JumpingState : IPlayerState
{
    private readonly PlayerController _player;
    private readonly PlayerConfig _config;
    private float _jumpTime;
    public PlayerStateType StateType => PlayerStateType.Jumping;

    public JumpingState(PlayerController player, PlayerConfig config)
    {
        _player = player;
        _config = config;
    }

    public void Enter()
    {
        _player.SetAnimationTrigger("Jump");
        _player.BeginJump();
        _jumpTime = Time.time;
    }

    public void Update()
    {
        float jumpDuration = Mathf.Max(_config.JumpDuration, 0.01f);
        float progress = (Time.time - _jumpTime) / jumpDuration;

        _player.SetJumpProgress(progress);

        if (progress >= 1f)
        {
            _player.StateMachine.ChangeState(PlayerStateType.Running);
        }
    }

    public void Exit()
    {
        _player.EndJump();
    }
}
