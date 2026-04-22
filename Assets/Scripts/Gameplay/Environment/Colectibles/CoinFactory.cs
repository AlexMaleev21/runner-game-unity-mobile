using UnityEngine;
using Zenject;

public class CoinFactory
{
    private readonly DiContainer _container;
    private readonly CoinSpawnConfig _config;

    public CoinFactory(DiContainer container, CoinSpawnConfig config)
    {
        _container = container;
        _config = config;
    }

    public Coin Create(Vector3 position)
    {
        var coin = _container.InstantiatePrefabForComponent<Coin>(_config.coinPrefab, position, Quaternion.identity, null);
        return coin;
    }
}