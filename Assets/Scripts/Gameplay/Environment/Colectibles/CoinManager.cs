using System;
using Zenject;
using Zenject.SpaceFighter;

public class CoinManager : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly InGameUI _inGameUI;
    private int _currentCoins;

    public CoinManager(SignalBus signalBus, InGameUI inGameUI)
    {
        _signalBus = signalBus;
        _inGameUI = inGameUI;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<CoinCollectedSignal>(OnCoinCollected);
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        ResetCoins();
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<CoinCollectedSignal>(OnCoinCollected);
        _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    private void OnCoinCollected()
    {
        _currentCoins++;
        _inGameUI.UpdateCoinText(_currentCoins);
    }

    private void OnPlayerDied()
    {
        ResetCoins();
    }

    private void ResetCoins()
    {
        _currentCoins = 0;
        _inGameUI.UpdateCoinText(0);
    }
}
