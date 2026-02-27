using UnityEngine;

public class IdleState : IPlayerState
{
    private readonly PlayerController _player;

    public PlayerStateType StateType => PlayerStateType.Idle;

    public IdleState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.SetAnimation("Idle", true);
    }

    public void Update() { }

    public void Exit()
    {
        _player.SetAnimation("Idle", false);
    }
}
