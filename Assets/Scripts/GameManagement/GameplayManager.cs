using Cysharp.Threading.Tasks;
using GoogleMobileAds.Ump.Api;
using System;
using UnityEngine;
using Zenject;

public class GameplayManager : IInitializable
{
    public event Action OnExitToMenuRequested;
    public event Action OnRestartRequested;

    private SignalBus _signalBus;
    private readonly PlayerController _player;
    private readonly ObstacleSpawner _spawner;
    private readonly ObstacleManipulator _obstacleMover;
    private readonly BackgroundMover _backgroundMover;
    private readonly SpeedManager _speedManager;
    private readonly ScoreManager _scoreManager;
    private readonly AdsManager _adsManager;
    private readonly ILeaderboardService _leaderboardService;

    private InGameUI _inGameUI;
    private GameOverWindow _gameOverWindow;


    public GameplayManager(
        PlayerController player,
        ObstacleSpawner spawner,
        ObstacleManipulator obstacleMover,
        SpeedManager speedManager,
        ScoreManager scoreManager,
        InGameUI inGameUI,
        GameOverWindow gameOverWindow,
        AdsManager adsManager,
        ILeaderboardService leaderboardService,
        SignalBus signalBus,
        BackgroundMover backgroundMover)
    {
        _player = player;
        _spawner = spawner;
        _obstacleMover = obstacleMover;
        _speedManager = speedManager;
        _scoreManager = scoreManager;
        _inGameUI = inGameUI;
        _gameOverWindow = gameOverWindow;
        _adsManager = adsManager;
        _leaderboardService = leaderboardService;
        _signalBus = signalBus;
        _backgroundMover = backgroundMover;
    }

    public void Initialize()
    {
        _adsManager.OnRewardedAdCompleted += OnAdRewarded;
        _inGameUI.Hide();
        _gameOverWindow.Hide();
        _player.StateMachine.ChangeState(PlayerStateType.Idle);
    }

    public void StartGame()
    {
        _obstacleMover.ClearAllObstacles();
        _obstacleMover.ResetGame();
        _spawner.ResetSpawner();

        _player.SetEnabled(true);
        _player.StateMachine.ChangeState(PlayerStateType.Running);
        _spawner.enabled = true;

        _backgroundMover.StartMovement();

        _speedManager.ResetSpeed();
        _scoreManager.ResetScore();

        _inGameUI.Show();
    }

    public void RestartGame()
    {

        _gameOverWindow.Hide();

        _obstacleMover.ClearAllObstacles();
        _obstacleMover.ResumeGame();
        _spawner.ResetSpawner();
        _speedManager.ResetSpeed();
        _scoreManager.ResetScore();
        _backgroundMover.ResetBackground();
        _backgroundMover.StartMovement();

        _player.ResetToStart();
        _player.SetEnabled(true);
        _player.StateMachine.ChangeState(PlayerStateType.Running);
        _spawner.enabled = true;

        _inGameUI.Show();
    }

    public void ExitToMenu()
    {
        _obstacleMover.ClearAllObstacles();
        _spawner.ResetSpawner();

        _player.StateMachine.ChangeState(PlayerStateType.Idle);
        _player.SetEnabled(false);
        _player.ResetToStart();
        _spawner.enabled = false;

        _gameOverWindow.Hide();
        OnExitToMenuRequested?.Invoke();
    }

    public async void OnPlayerDied()
    {
        _backgroundMover.StopMovement();
        _player.SetEnabled(false);
        _spawner.enabled = false;

        _gameOverWindow.OnRestart += () => OnRestartRequested?.Invoke();
        _gameOverWindow.OnExit += () => ExitToMenu();
        _gameOverWindow.OnWatchAd += OnWatchAdClicked;
        _gameOverWindow.Show();

        await _leaderboardService.SubmitScore(_scoreManager.CurrentScore);

        _inGameUI.Hide();
    }

    private void OnWatchAdClicked()
    {
        _adsManager.ShowRewardedAd();
    }

    private async void OnAdRewarded()
    {

        _obstacleMover.RemoveClosestObstacle(_player.transform.position.z);

        _spawner.PauseSpawn(2.5f);

        _player.SetEnabled(false);

        _gameOverWindow?.Hide();
        await UniTask.Delay(2500);

        _signalBus.Fire(new GameResumedSignal());
        _player.Ressurect();
        _player.SetEnabled(true);
        _player.StateMachine.ChangeState(PlayerStateType.Running);
        _obstacleMover.ResumeGame();
        _speedManager.Resume();

        _scoreManager.ResumeScore();
        _inGameUI?.Show();

    }
}
