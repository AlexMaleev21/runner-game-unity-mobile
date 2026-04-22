using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CoinMover : ITickable
{
    private readonly SpeedManager _speedManager;
    private readonly CoinPool _coinPool;
    private readonly List<Coin> _activeCoins = new List<Coin>();
    private bool _isActive = false;

    public CoinMover(SpeedManager speedManager, CoinPool coinPool)
    {
        _speedManager = speedManager;
        _coinPool = coinPool;
    }

    public void StartMovement() { _isActive = true; }
    public void StopMovement() { _isActive = false; }

    public void RegisterCoin(Coin coin) => _activeCoins.Add(coin);

    public void ClearAllCoins()
    {
        foreach (var coin in _activeCoins)
            if (coin != null) _coinPool.Return(coin);
        _activeCoins.Clear();
    }

    public void Tick()
    {
        if (!_isActive) return;
        float speed = _speedManager.CurrentSpeed;
        for (int i = _activeCoins.Count - 1; i >= 0; i--)
        {
            var coin = _activeCoins[i];
            if (coin == null) continue;
            coin.transform.Translate(Vector3.back * speed * Time.deltaTime);
            if (coin.transform.position.z < -20f)
            {
                _coinPool.Return(coin);
                _activeCoins.RemoveAt(i);
            }
        }
    }
}