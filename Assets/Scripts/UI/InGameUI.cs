using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _scoreText;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        _signalBus.Subscribe<ScoreUpdatedSignal>(OnScoreUpdated);
    }

    private void OnScoreUpdated(ScoreUpdatedSignal signal)
    {
        if (_scoreText != null)
            _scoreText.text = $"Score: {signal.Score}";
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
    }
}
