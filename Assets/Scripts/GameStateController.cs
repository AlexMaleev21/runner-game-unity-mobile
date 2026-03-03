using System;
using UnityEngine;
using Zenject;
using Zenject.SpaceFighter;

public enum GameState { Auth, Menu, Playing, GameOver }

public class GameStateController : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly AuthManager _authManager;
    private readonly MenuManager _menuManager;
    private readonly GameplayManager _gameplayManager;

    private GameState _currentState;

    public GameStateController(
        SignalBus signalBus,
        AuthManager authManager,
        MenuManager menuManager,
        GameplayManager gameplayManager)
    {
        _signalBus = signalBus;
        _authManager = authManager;
        _menuManager = menuManager;
        _gameplayManager = gameplayManager;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus.Subscribe<AuthSuccessSignal>(OnAuthSuccess);

        _menuManager.OnStartGameRequested += StartGame;
        _menuManager.OnLogoutRequested += Logout;

        _gameplayManager.OnRestartRequested += RestartGame;
        _gameplayManager.OnExitToMenuRequested += HandleExitToMenu;

        CheckAutoLogin();
    }

    private async void CheckAutoLogin()
    {
        _currentState = GameState.Auth;
        bool autoLoginSuccess = await _authManager.AutoLogin();
        if (autoLoginSuccess)
        {
            OnAuthSuccess();
        }
        else
        {
            _authManager.ShowAuth();
        }
    }

    private void OnAuthSuccess()
    {
        _authManager.HideAuth();
        _currentState = GameState.Menu;
        _menuManager.ShowMenu();
    }

    private void Logout()
    {
        _authManager.Logout();
        _menuManager.HideMenu();
        _currentState = GameState.Auth;
        _authManager.ShowAuth();
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

    private void OnPlayerDied()
    {
        if (_currentState != GameState.Playing) return;
        _currentState = GameState.GameOver;
        _gameplayManager.OnPlayerDied();
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        _signalBus?.Unsubscribe<AuthSuccessSignal>(OnAuthSuccess);

        _menuManager.OnStartGameRequested -= StartGame;
        _menuManager.OnLogoutRequested -= Logout;

        _gameplayManager.OnRestartRequested -= RestartGame;
        _gameplayManager.OnExitToMenuRequested -= HandleExitToMenu;
    }
}