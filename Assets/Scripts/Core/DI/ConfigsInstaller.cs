using UnityEngine;
using Zenject;

public class ConfigsInstaller : MonoInstaller
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private ObstacleSpawnConfig _obstacleSpawnConfig;
    [SerializeField] private CoinSpawnConfig _coinSpawnConfig;
    [SerializeField] private GameConfig _gameConfig;

    public override void InstallBindings()
    {
        Container.BindInstance(_playerConfig);
        Container.BindInstance(_obstacleSpawnConfig);
        Container.BindInstance(_coinSpawnConfig != null
            ? _coinSpawnConfig
            : Resources.Load<CoinSpawnConfig>("Configs/CoinSpawnConfig"));
        Container.BindInstance(_gameConfig);
    }
}
