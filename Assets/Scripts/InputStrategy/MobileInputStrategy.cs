using System;
using UnityEngine;

public class MobileInputStrategy : IInputStrategy
{
    public event Action<InputAction> OnInputPerformed;

    private Vector2 _touchStartPos;
    private float _swipeThreshold = 50f;
    private bool _isSwiping = false;

    public void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPos = touch.position;
                    _isSwiping = true;
                    break;

                case TouchPhase.Ended:
                    if (_isSwiping)
                    {
                        Vector2 swipeDelta = touch.position - _touchStartPos;

                        if (swipeDelta.magnitude > _swipeThreshold)
                        {
                            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                            {
                                if (swipeDelta.x > 0)
                                    OnInputPerformed?.Invoke(InputAction.Right);
                                else
                                    OnInputPerformed?.Invoke(InputAction.Left);
                            }
                            else
                            {
                                if (swipeDelta.y > 0)
                                    OnInputPerformed?.Invoke(InputAction.Jump);
                            }
                        }
                        else
                        {
                            OnInputPerformed?.Invoke(InputAction.Tap);
                        }
                    }
                    _isSwiping = false;
                    break;
            }
        }
    }
}
