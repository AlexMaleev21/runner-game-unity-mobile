using UnityEngine;

public class SlidingState : IPlayerState
{
    private readonly PlayerController _player;
    private readonly PlayerConfig _config;
    private float _slideStartTime;

    public PlayerStateType StateType => PlayerStateType.Sliding;

    public SlidingState(PlayerController player, PlayerConfig config)
    {
        _player = player;
        _config = config;
    }

    public void Enter()
    {
        _player.SetAnimationTrigger("Slide");
        _player.SetColliderHeight(_config.SlideHeight);
        _slideStartTime = Time.time;
    }

    public void Update()
    {
        if (Time.time > _slideStartTime + _config.SlideDuration)
        {
            _player.StateMachine.ChangeState(PlayerStateType.Running);
        }
    }

    public void Exit()
    {
        _player.SetColliderHeight(_config.NormalHeight);
    }
}
