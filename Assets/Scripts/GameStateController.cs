using System;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public enum GameState { Nickname, Menu, Playing, GameOver }

public class GameStateController : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly NicknamePromptWindow _nicknamePromptWindow;
    private readonly ILeaderboardService _leaderboardService;
    private readonly MenuManager _menuManager;
    private readonly GameplayManager _gameplayManager;
    private readonly BackgroundMover _backgroundMover;

    private GameState _currentState;

    public GameStateController(
        SignalBus signalBus,
        NicknamePromptWindow nicknamePromptWindow,
        ILeaderboardService leaderboardService,
        MenuManager menuManager,
        GameplayManager gameplayManager,
        BackgroundMover backgroundMover)
    {
        _signalBus = signalBus;
        _nicknamePromptWindow = nicknamePromptWindow;
        _leaderboardService = leaderboardService;
        _menuManager = menuManager;
        _gameplayManager = gameplayManager;
        _backgroundMover = backgroundMover;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Subscribe<GameResumedSignal>(OnGameResumed);

        _menuManager.OnStartGameRequested += StartGame;

        _gameplayManager.OnRestartRequested += RestartGame;
        _gameplayManager.OnExitToMenuRequested += HandleExitToMenu;

        ShowNicknamePrompt();
    }

    private void ShowNicknamePrompt()
    {
        _currentState = GameState.Nickname;
        _nicknamePromptWindow.Show(ShowMenu);
    }

    private async void ShowMenu()
    {
        await _leaderboardService.RegisterNickname();
        _currentState = GameState.Menu;
        _menuManager.ShowMenu();
    }

    private void StartGame()
    {
        _currentState = GameState.Playing;
        _menuManager.HideMenu();
        _gameplayManager.StartGame();
    }

    private void RestartGame()
    {
        _currentState = GameState.Playing;
        _gameplayManager.RestartGame();
    }

    private void HandleExitToMenu()
    {
        _currentState = GameState.Menu;
        _menuManager.ShowMenu();
    }

    private void OnGameResumed()
    {
        _currentState = GameState.Playing;
    }

    private void OnPlayerDied()
    {
        if (_currentState != GameState.Playing) return;
        _currentState = GameState.GameOver;
        _gameplayManager.OnPlayerDied();
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus?.Unsubscribe<GameResumedSignal>(OnGameResumed);

        _menuManager.OnStartGameRequested -= StartGame;

        _gameplayManager.OnRestartRequested -= RestartGame;
        _gameplayManager.OnExitToMenuRequested -= HandleExitToMenu;
    }
}

public struct GameResumedSignal { }
