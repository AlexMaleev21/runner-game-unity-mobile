using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class PlayerController : MonoBehaviour
{
    private IInputHandler _inputHandler;
    private PlayerConfig _config;
    private PlayerStateMachine _stateMachine;
    private SignalBus _signalBus;
    private Transform _playerSpawnPoint;

    private Rigidbody _rigidbody;
    private Collider _playerCollider;
    private Animator _animator;
    private int _currentLane = 0;
    private float _targetX;
    private float _baseY;
    private bool _isDead = false;

    public PlayerStateMachine StateMachine => _stateMachine;

    [Inject]
    public void Construct(
        IInputHandler inputHandler,
        PlayerConfig config,
        PlayerStateMachine stateMachine,
        SignalBus signalBus,
        Transform playerSpawnPoint)
    {
        _inputHandler = inputHandler;
        _config = config;
        _stateMachine = stateMachine;
        _signalBus = signalBus;
        _playerSpawnPoint = playerSpawnPoint;
    }

    private void Awake()
    {
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _playerCollider = GetComponentInChildren<Collider>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        _stateMachine.ChangeState(PlayerStateType.Idle);
        transform.position = _playerSpawnPoint.position;
        _baseY = transform.position.y;
        _inputHandler.OnInput += HandleInput;
    }

    private void Update()
    {
        if (_isDead || !enabled) return;

        _stateMachine.Update();

        _targetX = _currentLane * _config.LaneWidth;

        Vector3 targetPosition = new Vector3(
            _targetX,
            transform.position.y,
            transform.position.z
        );
        transform.position = Vector3.Lerp(transform.position, targetPosition, _config.LaneSwitchSpeed * Time.deltaTime);
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
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, _config.GroundLayer);
    }

    public float GetAnimationDuration(string clipName)
    {
        if (_animator == null)
            return 0f;

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

    public void BeginJump()
    {
        _baseY = transform.position.y;
        ResetPhysicsVelocity();
    }

    public void SetJumpProgress(float normalizedProgress)
    {
        float clampedProgress = Mathf.Clamp01(normalizedProgress);
        float jumpOffset = Mathf.Sin(clampedProgress * Mathf.PI) * _config.JumpHeight;
        Vector3 position = transform.position;
        position.y = _baseY + jumpOffset;
        transform.position = position;
    }

    public void EndJump()
    {
        ResetPhysicsVelocity();
        Vector3 position = transform.position;
        position.y = _baseY;
        transform.position = position;
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
        if (_rigidbody != null)
        {
            ResetPhysicsVelocity();
            _rigidbody.isKinematic = true;
        }
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
        _baseY = transform.position.y;
        _currentLane = 0;
        _targetX = 0;
        _isDead = false;
        if (_rigidbody != null)
        {
            ResetPhysicsVelocity();
            _rigidbody.isKinematic = false;
        }
    }

    private void ResetPhysicsVelocity()
    {
        if (_rigidbody == null)
            return;

#if UNITY_6000_0_OR_NEWER
        _rigidbody.linearVelocity = Vector3.zero;
#else
        _rigidbody.velocity = Vector3.zero;
#endif
        _rigidbody.angularVelocity = Vector3.zero;
    }

    private void OnDestroy()
    {
        if (_inputHandler != null)
            _inputHandler.OnInput -= HandleInput;
    }
}
