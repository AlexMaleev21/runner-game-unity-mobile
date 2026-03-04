using UnityEngine;
using Zenject;

public class ConfigsInstaller : MonoInstaller
{
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private ObstacleSpawnConfig _obstacleSpawnConfig;
    [SerializeField] private GameConfig _gameConfig;

    public override void InstallBindings()
    {
        Container.BindInstance(_playerConfig);
        Container.BindInstance(_obstacleSpawnConfig);
        Container.BindInstance(_gameConfig);
    }
}
