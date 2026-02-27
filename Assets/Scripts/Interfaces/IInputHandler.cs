using System;

public interface IInputHandler
{
    event Action<InputAction> OnInput;
    void Update();
}