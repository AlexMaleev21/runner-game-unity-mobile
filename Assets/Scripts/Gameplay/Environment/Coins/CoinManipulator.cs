using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class CoinManipulator : MonoBehaviour
{
    private readonly List<Coin> _activeCoins = new List<Coin>();

    private CoinPool _coinPool;
    private SignalBus _signalBus;
    private float _speed;
    private bool _isGameActive;
    private int _collectedCoins;

    [Inject]
    public void Construct(CoinPool coinPool, SignalBus signalBus)
    {
        _coinPool = coinPool;
        _signalBus = signalBus;
    }

    private void Start()
    {
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
    }

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void RegisterCoin(Coin coin)
    {
        if (coin == null || _activeCoins.Contains(coin))
            return;

        coin.Collected += OnCoinCollected;
        _activeCoins.Add(coin);
    }

    public void ClearAllCoins()
    {
        for (int i = _activeCoins.Count - 1; i >= 0; i--)
        {
            ReturnCoin(_activeCoins[i]);
        }

        _activeCoins.Clear();
    }

    public void ResetGame()
    {
        _collectedCoins = 0;
        _signalBus.Fire(new CoinsUpdatedSignal { Coins = _collectedCoins });
        _isGameActive = true;
    }

    public void ResumeGame()
    {
        _isGameActive = true;
    }

    private void Update()
    {
        if (!_isGameActive)
            return;

        for (int i = _activeCoins.Count - 1; i >= 0; i--)
        {
            Coin coin = _activeCoins[i];
            if (coin == null)
            {
                _activeCoins.RemoveAt(i);
                continue;
            }

            coin.transform.position -= new Vector3(0f, 0f, _speed * Time.deltaTime);

            if (coin.transform.position.z < -20f)
                ReturnCoin(coin);
        }
    }

    private void OnCoinCollected(Coin coin)
    {
        _collectedCoins++;
        _signalBus.Fire(new CoinsUpdatedSignal { Coins = _collectedCoins });
        ReturnCoin(coin);
    }

    private void ReturnCoin(Coin coin)
    {
        if (coin == null)
            return;

        coin.Collected -= OnCoinCollected;
        _activeCoins.Remove(coin);
        _coinPool.Return(coin);
    }

    private void OnPlayerDied()
    {
        _isGameActive = false;
    }

    private void OnDestroy()
    {
        _signalBus?.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
    }
}

public struct CoinsUpdatedSignal
{
    public int Coins;
}
