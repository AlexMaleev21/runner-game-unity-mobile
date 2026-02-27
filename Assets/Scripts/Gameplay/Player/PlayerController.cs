using System;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class PlayerController : MonoBehaviour
{
    //
    [Inject] private IInputHandler _inputHandler;
    [Inject] private PlayerConfig _config;
    [Inject] private PlayerStateMachine _stateMachine;
    [Inject] private SignalBus _signalBus;
    [Inject] private Transform _playerSpawnPoint;

    private Rigidbody _rigidbody;
    private Collider _playerCollider;
    private Animator _animator;
    private int _currentLane = 0;
    private float _targetX;
    private bool _isDead = false;
    private Vector3 _startPosition;

    public PlayerStateMachine StateMachine => _stateMachine;


    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _playerCollider = GetComponent<Collider>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _stateMachine.ChangeState(PlayerStateType.Idle);
        transform.position = _playerSpawnPoint.position;
        _startPosition = transform.position;
        _inputHandler.OnInput += HandleInput;
    }

    private void Update()
    {
        if (_isDead || !enabled) return;

        _stateMachine.Update();

        _targetX = _currentLane * _config.laneWidth;

        Vector3 targetPosition = new Vector3(
            _targetX,
            transform.position.y,
            transform.position.z
        );
        transform.position = Vector3.Lerp(transform.position, targetPosition, _config.laneSwitchSpeed * Time.deltaTime);
    }

    private void HandleInput(InputAction action)
    {
        if (_isDead || !enabled || _stateMachine.CurrentState.StateType == PlayerStateType.Dead)
            return;

        switch (action)
        {
            case InputAction.Left:
                if (_currentLane > -1) _currentLane--;
                break;

            case InputAction.Right:
                if (_currentLane < 1) _currentLane++;
                break;

            case InputAction.Jump:
                if (_stateMachine.CurrentState.StateType == PlayerStateType.Running)
                {
                    _stateMachine.ChangeState(PlayerStateType.Jumping);
                }
                break;

            case InputAction.Slide:
                if (_stateMachine.CurrentState.StateType == PlayerStateType.Running)
                    _stateMachine.ChangeState(PlayerStateType.Sliding);
                break;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, _config.groundLayer);
    }

    public float GetAnimationDuration(string clipName)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float duration = stateInfo.length;
        return duration;
    }
    public void SetAnimation(string animationName, bool value)
    {
        if (_animator != null)
            _animator.SetBool(animationName, value);
    }

    public void SetAnimationTrigger(string triggerName)
    {
        if (_animator != null)
            _animator.SetTrigger(triggerName);
    }

    public void SetColliderHeight(float height)
    {
        if (_playerCollider is CapsuleCollider capsule)
        {
            capsule.height = height;
            capsule.center = new Vector3(0, height / 2f, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") && !_isDead && enabled)
        {
            Die();
        }
    }

    public void Die()
    {
        _isDead = true;
        _stateMachine.ChangeState(PlayerStateType.Dead);
        _signalBus.Fire(new PlayerDiedSignal());
    }

    public void Ressurect()
    {
        _isDead = false;
    }
    public void OnPlayerDied()
    {
        _rigidbody.isKinematic = true;
    }

    public void SetEnabled(bool enabled)
    {
        this.enabled = enabled;
        if (_rigidbody != null)
        {
            if (!enabled)
            {
                _rigidbody.isKinematic = true;
            }
            else
            {
                _rigidbody.isKinematic = false;
            }
        }
    }

    public void ResetToStart()
    {
        transform.position = _playerSpawnPoint.position;
        _currentLane = 0;
        _targetX = 0;
        _isDead = false;
        _rigidbody.isKinematic = false;
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
            _inputHandler.OnInput -= HandleInput;
    }
}