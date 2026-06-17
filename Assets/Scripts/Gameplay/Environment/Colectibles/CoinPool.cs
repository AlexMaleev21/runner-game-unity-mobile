using System.Collections.Generic;
using UnityEngine;

public class CoinPool
{
    private readonly Queue<Coin> _pool = new Queue<Coin>();
    private readonly CoinFactory _factory;
    private readonly CoinSpawnConfig _config;
    private readonly Transform _poolParent;

    public CoinPool(CoinFactory factory, CoinSpawnConfig config)
    {
        _factory = factory;
        _config = config;
        _poolParent = new GameObject("CoinPool").transform;

        for (int i = 0; i < _config.poolSize; i++)
        {
            var coin = _factory.Create(Vector3.zero);
            coin.transform.SetParent(_poolParent);
            coin.gameObject.SetActive(false);
            _pool.Enqueue(coin);
        }
    }

    public Coin Get(Vector3 position)
    {
        Coin coin;
        if (_pool.Count > 0)
        {
            coin = _pool.Dequeue();
            coin.transform.position = position;
            coin.gameObject.SetActive(true);
        }
        else
        {
            coin = _factory.Create(position);
        }
        return coin;
    }

    public void Return(Coin coin)
    {
        coin.gameObject.SetActive(false);
        coin.transform.SetParent(_poolParent);
        _pool.Enqueue(coin);
    }
}