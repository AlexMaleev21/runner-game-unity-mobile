using System;
using UnityEngine;
using Zenject;

public class MenuManager : IInitializable
{
    public event Action OnStartGameRequested;
    public event Action OnLogoutRequested;

    private readonly LeaderboardWindow _leaderboardWindow;
    private MainMenuWindow _mainMenu;

    public MenuManager(
        MainMenuWindow mainMenu,
        LeaderboardWindow leaderboardWindow)
    {
        _mainMenu = mainMenu;
        _leaderboardWindow = leaderboardWindow;
    }

    public void Initialize()
    {
        //HideMenu();
    }

    public void ShowMenu()
    {
        _mainMenu.OnStartGame += () => OnStartGameRequested?.Invoke();
        _mainMenu.OnLogout += () => OnLogoutRequested?.Invoke();
        _mainMenu.OnLeaderboard += ShowLeaderboard;
        _mainMenu.OnQuit += QuitGame;

        _mainMenu.Show();
    }

    private void ShowLeaderboard()
    {
        //_leaderboardWindow.UpdateLeaderboard();
        _leaderboardWindow.Show();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void HideMenu()
    {
        _mainMenu.OnStartGame -= () => OnStartGameRequested?.Invoke();
        _mainMenu.OnLogout -= () => OnLogoutRequested?.Invoke();
        _mainMenu.OnLeaderboard -= ShowLeaderboard;
        _mainMenu.OnQuit -= QuitGame;

        _mainMenu.Hide();
    }
}
