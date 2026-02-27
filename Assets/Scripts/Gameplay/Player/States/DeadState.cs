public class DeadState : IPlayerState
{
    private readonly PlayerController _player;

    public PlayerStateType StateType => PlayerStateType.Dead;

    public DeadState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.OnPlayerDied();
    }

    public void Update() { }

    public void Exit()
    {

    }
}
