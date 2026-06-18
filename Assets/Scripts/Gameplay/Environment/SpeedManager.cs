using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class SpeedManager : ITickable
{
    private readonly ObstacleManipulator _obstacleMover;
    private readonly CoinManipulator _coinMover;
    private readonly GameConfig _config;
    private readonly SignalBus _signalBus;
    
    private float _currentSpeed;
    public float CurrentSpeed { get; private set; }

    private bool _isGameActive = true;

    public SpeedManager(
        ObstacleManipulator obstacleMover,
        CoinManipulator coinMover,
        GameConfig config,
        SignalBus signalBus)
    {
        _obstacleMover = obstacleMover;
        _coinMover = coinMover;
        _config = config;
        _signalBus = signalBus;
        CurrentSpeed = _config.InitialSpeed;
        _obstacleMover.SetSpeed(CurrentSpeed);
        _coinMover.SetSpeed(CurrentSpeed);

        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    public void Tick()
    {
        if (!_isGameActive) return;
        CurrentSpeed += _config.SpeedIncreasePerSecond * Time.deltaTime;
        CurrentSpeed = Mathf.Min(CurrentSpeed, _config.MaxSpeed);
        _obstacleMover.SetSpeed(CurrentSpeed);
        _coinMover.SetSpeed(CurrentSpeed);
    }

    public void Resume()
    {
        _isGameActive = true;
    }

    private void OnPlayerDied()
    {
        _isGameActive = false;
    }

    public void ResetSpeed()
    {
        CurrentSpeed = _config.InitialSpeed;
        _obstacleMover.SetSpeed(CurrentSpeed);
        _coinMover.SetSpeed(CurrentSpeed);
        _isGameActive = true;
    }
}
