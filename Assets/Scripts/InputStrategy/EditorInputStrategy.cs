using System;
using UnityEngine;

public class EditorInputStrategy : IInputStrategy
{
    public event Action<InputAction> OnInputPerformed;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            OnInputPerformed?.Invoke(InputAction.Left);

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            OnInputPerformed?.Invoke(InputAction.Right);

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            OnInputPerformed?.Invoke(InputAction.Jump);
            Debug.Log("ww");
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            OnInputPerformed?.Invoke(InputAction.Slide);

        if (Input.GetMouseButtonDown(0))
            OnInputPerformed?.Invoke(InputAction.Tap);
    }
}