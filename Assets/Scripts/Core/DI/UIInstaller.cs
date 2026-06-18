using UnityEngine;
using Zenject;

public class UIInstaller : MonoInstaller
{
    [SerializeField] private GameObject _mainMenuWindowPrefab;
    [SerializeField] private GameObject _gameOverWindowPrefab;
    [SerializeField] private GameObject _leaderboardWindowPrefab;
    [SerializeField] private GameObject _inGameUIPrefab;

    public override void InstallBindings()
    {
        Container.Bind<MainMenuWindow>()
            .FromComponentInNewPrefab(_mainMenuWindowPrefab)
            .UnderTransformGroup("UI")
            .AsSingle();

        Container.Bind<GameOverWindow>()
            .FromComponentInNewPrefab(_gameOverWindowPrefab)
            .UnderTransformGroup("UI")
            .AsSingle();

        Container.Bind<LeaderboardWindow>()
        .FromComponentInNewPrefab(_leaderboardWindowPrefab)
        .UnderTransformGroup("UI")
        .AsSingle();
        Container.Bind<InGameUI>()
        .FromComponentInNewPrefab(_inGameUIPrefab)
        .UnderTransformGroup("UI")
        .AsSingle();
    }
}
