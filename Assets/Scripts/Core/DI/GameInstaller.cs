using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public class GameInstaller : MonoInstaller
{
    [Header("Configs")]
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private ObstacleSpawnConfig _obstacleSpawnConfig;
    [SerializeField] private GameConfig _gameConfig;

    [Header("Prefabs")]
    [SerializeField] private PlayerController _playerPrefab;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _authWindowPrefab;
    [SerializeField] private GameObject _mainMenuWindowPrefab;
    [SerializeField] private GameObject _gameOverWindowPrefab;
    [SerializeField] private GameObject _inGameUIPrefab;

    public override void InstallBindings()
    {
        InstallConfigs();
        InstallSignals();
        InstallInput();
        InstallAuth();
        InstallAds();
        InstallPlayer();
        InstallObstacles();
        InstallManagers();
        InstallUI();
    }

    private void InstallConfigs()
    {
        Container.BindInstance(_playerConfig);
        Container.BindInstance(_obstacleSpawnConfig);
        Container.BindInstance(_gameConfig);
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<PlayerDiedSignal>();
        Container.DeclareSignal<ScoreUpdatedSignal>();
        Container.DeclareSignal<AuthSuccessSignal>();
    }

    private void InstallInput()
    {
#if UNITY_EDITOR
        Container.Bind<IInputStrategy>().To<EditorInputStrategy>().AsSingle();
#else
        Container.Bind<IInputStrategy>().To<MobileInputStrategy>().AsSingle();
#endif
        Container.BindInterfacesTo<InputHandler>().AsSingle();
    }

    private void InstallAuth()
    {
        Container.Bind<IAuthService>().To<FirebaseAuthService>().AsSingle();
    }

    private void InstallAds()
    {
        Container.Bind<AdmobAdsService>().AsSingle();
    }

    private void InstallPlayer()
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

    private void InstallObstacles()
    {
        Container.Bind<IObstacleFactory>().To<ObstacleFactory>().AsSingle();
        Container.Bind<ObstaclePool>().AsSingle();
        Container.Bind<ObstacleMover>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ObstacleSpawner>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }

    private void InstallManagers()
    {
        Container.BindInterfacesAndSelfTo<SpeedManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<AuthManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<AdsManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<MenuManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplayManager>().AsSingle();
        Container.BindInterfacesTo<GameStateController>().AsSingle();
    }

    private void InstallUI()
    {
        Container.Bind<AuthWindow>()
            .FromComponentInNewPrefab(_authWindowPrefab)
            .UnderTransformGroup("UI")
            .AsSingle();

        Container.Bind<MainMenuWindow>()
            .FromComponentInNewPrefab(_mainMenuWindowPrefab)
            .UnderTransformGroup("UI")
            .AsSingle();

        Container.Bind<GameOverWindow>()
            .FromComponentInNewPrefab(_gameOverWindowPrefab)
            .UnderTransformGroup("UI")
            .AsSingle();
        
        Container.Bind<InGameUI>()
            .FromComponentInNewPrefab(_inGameUIPrefab)
            .UnderTransformGroup("UI")
            .AsSingle();

    }
}