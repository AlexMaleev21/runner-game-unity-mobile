using TMPro;
using UnityEngine;
using Zenject;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _coinsText;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Start()
    {
        _signalBus.Subscribe<ScoreUpdatedSignal>(OnScoreUpdated);
        _signalBus.Subscribe<CoinsUpdatedSignal>(OnCoinsUpdated);
        OnCoinsUpdated(new CoinsUpdatedSignal { Coins = 0 });
    }

    private void OnScoreUpdated(ScoreUpdatedSignal signal)
    {
        if (_scoreText != null)
            _scoreText.text = $"Score: {signal.Score}";
    }

    private void OnCoinsUpdated(CoinsUpdatedSignal signal)
    {
        if (_coinsText != null)
            _coinsText.text = $"Coins: {signal.Coins}";
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _signalBus.TryUnsubscribe<ScoreUpdatedSignal>(OnScoreUpdated);
        _signalBus.TryUnsubscribe<CoinsUpdatedSignal>(OnCoinsUpdated);
    }
}
