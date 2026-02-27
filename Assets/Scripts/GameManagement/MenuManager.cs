using System;
using Unity.VisualScripting;
using UnityEngine;

public class MenuManager : IInitializable
{
    public event Action OnStartGameRequested;
    public event Action OnLogoutRequested;

    private MainMenuWindow _mainMenu;

    public MenuManager(MainMenuWindow mainMenu)
    {
        _mainMenu = mainMenu;
    }

    public void Initialize()
    {
        HideMenu();
    }

    public void ShowMenu()
    {
        _mainMenu.OnStartGame += () => OnStartGameRequested?.Invoke();
        _mainMenu.OnLogout += () => OnLogoutRequested?.Invoke();
        _mainMenu.Show();
    }

    public void HideMenu()
    {
        _mainMenu.Hide();
    }
}
