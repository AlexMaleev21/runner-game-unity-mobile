using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MainMenuWindow : BaseWindow
{
    public event Action OnStartGame;
    public event Action OnLeaderboard;
    public event Action OnQuit;

    [SerializeField] private Button _logoutButton;
    [SerializeField] private Button _leaderboardButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        if (_logoutButton != null)
            _logoutButton.gameObject.SetActive(false);

        _leaderboardButton.onClick.AddListener(() => OnLeaderboard?.Invoke());
        _quitButton.onClick.AddListener(() => OnQuit?.Invoke());
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
