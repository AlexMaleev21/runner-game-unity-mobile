using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuWindow : BaseWindow
{
    public event Action OnStartGame;
    public event Action OnLogout;
    public event Action OnLeaderboard;
    public event Action OnQuit;

    [SerializeField] private Button _logoutButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _logoutButton.onClick.AddListener(OnLogoutClicked);
        _leaderboardButton.onClick.AddListener(() => OnLeaderboard?.Invoke());
        _quitButton.onClick.AddListener(() => OnQuit?.Invoke());
    }

    private void OnLogoutClicked()
    {
        OnLogout?.Invoke();
    }
    private bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current != null &&
               UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            OnStartGame?.Invoke();
        }
    }
}