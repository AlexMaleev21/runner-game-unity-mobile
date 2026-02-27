using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameplayManager : IInitializable
{
    public event Action OnExitToMenuRequested;
    public event Action OnRestartRequested;

    private readonly PlayerController _player;
    private readonly ObstacleSpawner _spawner;
    private readonly ObstacleMover _obstacleMover;
    private readonly SpeedManager _speedManager;
    private readonly ScoreManager _scoreManager;
    private readonly AdsManager _adsManager; 

    private InGameUI _inGameUI;
    private GameOverWindow _gameOverWindow;

    public GameplayManager(
        PlayerController player,
        ObstacleSpawner spawner,
        ObstacleMover obstacleMover,
        SpeedManager speedManager,
        ScoreManager scoreManager,
        InGameUI inGameUI,
        GameOverWindow gameOverWindow,
        AdsManager adsManager)
    {
        _player = player;
        _spawner = spawner;
        _obstacleMover = obstacleMover;
        _speedManager = speedManager;
        _scoreManager = scoreManager;
        _inGameUI = inGameUI;
        _gameOverWindow = gameOverWindow;
        _adsManager = adsManager;
    }

    public void Initialize()
    {
        Debug.Log(_adsManager.ToString());
        _adsManager.OnGameContinue += ContinueGameAfterAd;

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

    public void OnPlayerDied()
    {
        _player.SetEnabled(false);
        _spawner.enabled = false;

        _gameOverWindow.OnRestart += () => OnRestartRequested?.Invoke();
        _gameOverWindow.OnExit += () => ExitToMenu();
        _gameOverWindow.OnWatchAd += OnWatchAdClicked;
        _gameOverWindow.Show();

        _inGameUI.Hide();
    }

    private void OnWatchAdClicked()
    {
        _adsManager.ShowRewardedAd();
    }

    private void ContinueGameAfterAd()
    {
        _gameOverWindow.Hide();
        Debug.Log("Continue after ad");
        _inGameUI.Show();
    }
}
