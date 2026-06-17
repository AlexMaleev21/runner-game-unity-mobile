using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameOverWindow : BaseWindow
{
    public event Action OnRestart;
    public event Action OnExit;
    public event Action OnWatchAd;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _watchAdButton;
    [SerializeField] private TextMeshProUGUI _scoreText;

    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<ScoreUpdatedSignal>(OnScoreUpdated);
    }

    private void Start()
    {
        _restartButton.onClick.AddListener(() => OnRestart?.Invoke());
        _exitButton.onClick.AddListener(() => OnExit?.Invoke());
        _watchAdButton.onClick.AddListener(() => OnWatchAd?.Invoke());

    }

    private void OnScoreUpdated(ScoreUpdatedSignal signal)
    {
        _scoreText.text = $"Score: {signal.Score}";
    }

    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<ScoreUpdatedSignal>(OnScoreUpdated);
    }
}