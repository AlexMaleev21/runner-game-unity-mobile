public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
    PlayerStateType StateType { get; }
}

public enum PlayerStateType
{
    Idle,
    Running,
    Jumping,
    Sliding,
    Dead
}
