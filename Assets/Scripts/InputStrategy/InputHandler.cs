using System;
using UnityEngine;
using Zenject;

public class InputHandler : IInputHandler, ITickable
{
    public event Action<InputAction> OnInput;

    private readonly IInputStrategy _inputStrategy;

    public InputHandler(IInputStrategy inputStrategy)
    {
        _inputStrategy = inputStrategy;
        _inputStrategy.OnInputPerformed += HandleInput;
    }

    public void Tick()
    {
        _inputStrategy.Update();
    }

    private void HandleInput(InputAction action)
    {
        OnInput?.Invoke(action);
    }

    public void Update()
    {

    }
}
