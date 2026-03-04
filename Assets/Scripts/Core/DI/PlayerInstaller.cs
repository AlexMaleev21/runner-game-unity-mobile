using UnityEngine;
using Zenject;

public class PlayerInstaller : MonoInstaller
{
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private Transform _playerSpawnPoint;

    public override void InstallBindings()
    {
        Container.Bind<PlayerStateMachine>().AsSingle();
        Container.Bind<IdleState>().AsSingle();
        Container.Bind<RunningState>().AsSingle();
        Container.Bind<JumpingState>().AsSingle();
        Container.Bind<SlidingState>().AsSingle();
        Container.Bind<DeadState>().AsSingle();

        Container.BindInstance(_playerSpawnPoint);
        Container.Bind<PlayerController>()
            .FromComponentInNewPrefab(_playerPrefab)
            .AsSingle()
            .NonLazy();
    }
}