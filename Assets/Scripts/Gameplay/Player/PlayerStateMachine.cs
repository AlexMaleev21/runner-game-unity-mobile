using System;
using System.Collections.Generic;
using System.Xml;
using Zenject;
using UnityEngine;

public class PlayerStateMachine
{
    private IPlayerState _currentState;
    private readonly Dictionary<PlayerStateType, IPlayerState> _states;

    public IPlayerState CurrentState => _currentState;
    public event Action<PlayerStateType> OnStateChanged;

    public PlayerStateMachine(
        RunningState runningState,
        JumpingState jumpingState,
        SlidingState slidingState,
        DeadState deadState,
        IdleState idleState)
    {
        _states = new Dictionary<PlayerStateType, IPlayerState>
        {
            { PlayerStateType.Idle, idleState },
            { PlayerStateType.Running, runningState },
            { PlayerStateType.Jumping, jumpingState },
            { PlayerStateType.Sliding, slidingState },
            { PlayerStateType.Dead, deadState }
        };
    }

    public void ChangeState(PlayerStateType newStateType)
    {
        if (_currentState?.StateType == newStateType)
            return;

        _currentState?.Exit();
        _currentState = _states[newStateType];
        _currentState.Enter();
        OnStateChanged?.Invoke(newStateType);
    }

    public void Update()
    {
        _currentState?.Update();
    }
}