using System;

public interface IInputStrategy
{
    void Update();
    event Action<InputAction> OnInputPerformed;
}

public enum InputAction
{
    Left,
    Right,
    Jump,
    Slide,
    Tap
}