using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CoinPool
{
    private readonly DiContainer _container;
    private readonly CoinSpawnConfig _config;
    private readonly Queue<Coin> _pool = new Queue<Coin>();
    private readonly Transform _poolParent;

    public CoinPool(DiContainer container, CoinSpawnConfig config)
    {
        _container = container;
        _config = config;
        _poolParent = new GameObject("CoinPool").transform;

        for (int i = 0; i < _config.CoinPoolSize; i++)
        {
            Coin coin = Create(Vector3.zero);
            if (coin == null)
                continue;

            coin.transform.SetParent(_poolParent);
            coin.OnDespawn();
            _pool.Enqueue(coin);
        }
    }

    public Coin Get(Vector3 position)
    {
        Coin coin = _pool.Count > 0 ? _pool.Dequeue() : Create(position);
        if (coin == null)
            return null;

        coin.transform.position = position;
        coin.transform.SetParent(null);
        coin.OnSpawn();
        return coin;
    }

    public void Return(Coin coin)
    {
        if (coin == null)
            return;

        coin.OnDespawn();
        coin.transform.SetParent(_poolParent);
        _pool.Enqueue(coin);
    }

    private Coin Create(Vector3 position)
    {
        if (_config.CoinPrefab == null)
        {
            Debug.LogError("No coin prefab assigned in CoinSpawnConfig.");
            return null;
        }

        Coin coin = _container.InstantiatePrefabForComponent<Coin>(_config.CoinPrefab, position, Quaternion.identity, null);
        if (coin == null)
            Debug.LogError("Coin prefab must have a Coin component.");

        return coin;
    }
}
