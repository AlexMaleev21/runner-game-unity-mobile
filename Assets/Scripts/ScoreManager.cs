using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class ScoreManager : ITickable
{
    private readonly GameConfig _config;
    private readonly SignalBus _signalBus;

    private int _currentScore;
    private float _scoreAdd;
    private bool _isGameActive = false;

    public int CurrentScore => _currentScore;

    public ScoreManager(GameConfig config, SignalBus signalBus)
    {
        _config = config;
        _signalBus = signalBus;

        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    public void Tick()
    {
        if (!_isGameActive) return;

        _scoreAdd += Time.deltaTime * 10 * _config.scoreMultiplier;
        _currentScore = Mathf.RoundToInt(_scoreAdd);
        _signalBus.Fire(new ScoreUpdatedSignal { Score = _currentScore });
    }

    private void OnPlayerDied()
    {
        _isGameActive = false;

    }

    public void ResumeScore()
    {
        _isGameActive = true;
    }
    public void ResetScore()
    {
        _scoreAdd = 0;
        _currentScore = 0;
        _isGameActive = true;
    }
}

public struct ScoreUpdatedSignal
{
    public int Score;
}
