using UnityEngine;

public class SlidingState : IPlayerState
{
    private readonly PlayerController _player;
    private float _slideStartTime;
    private float _stateDuration;
    private const float DefaultStateDuration = 0.75f;

    public PlayerStateType StateType => PlayerStateType.Sliding;

    public SlidingState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimationTrigger("Slide");
        _slideStartTime = Time.time;
        _stateDuration = Mathf.Max(_player.GetAnimationDuration("Slide"), DefaultStateDuration);
    }

    public void Update()
    {
        if (Time.time > _slideStartTime + _stateDuration)
        {
            _player.StateMachine.ChangeState(PlayerStateType.Running);
        }
    }

    public void Exit()
    {
    }
}
