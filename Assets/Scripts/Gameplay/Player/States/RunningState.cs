using UnityEngine;

public class RunningState : IPlayerState
{
    private readonly PlayerController _player;
    private readonly PlayerConfig _config;

    public PlayerStateType StateType => PlayerStateType.Running;

    public RunningState(PlayerController player, PlayerConfig config)
    {
        _player = player;
        _config = config;
    }

    public void Enter()
    {
        _player.SetAnimation("Running", true);
        _player.SetColliderHeight(_config.normalHeight);
    }

    public void Update()
    {
        if (_player.transform.position.y < -5f)
        {
            _player.Die();
        }
    }

    public void Exit()
    {
        _player.SetAnimation("Running", false);
    }
}
