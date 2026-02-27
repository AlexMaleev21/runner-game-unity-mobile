using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class SpeedManager : ITickable
{
    private readonly ObstacleMover _obstacleMover;
    private readonly GameConfig _config;
    private readonly SignalBus _signalBus;
    private float _currentSpeed;
    private bool _isGameActive = true;

    public SpeedManager(ObstacleMover obstacleMover, GameConfig config, SignalBus signalBus)
    {
        _obstacleMover = obstacleMover;
        _config = config;
        _signalBus = signalBus;
        _currentSpeed = _config.initialSpeed;
        _obstacleMover.SetSpeed(_currentSpeed);

        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    public void Tick()
    {
        if (!_isGameActive) return;
        _currentSpeed += _config.speedIncreasePerSecond * Time.deltaTime;
        _currentSpeed = Mathf.Min(_currentSpeed, _config.maxSpeed);
        _obstacleMover.SetSpeed(_currentSpeed);
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
        _currentSpeed = _config.initialSpeed;
        _obstacleMover.SetSpeed(_currentSpeed);
        _isGameActive = true;
    }
}