using System;
using UnityEngine;
using Zenject;

public class NicknamePromptWindow : MonoBehaviour
{
    private const int WindowWidth = 360;
    private const int WindowHeight = 170;

    private PlayerProfileService _playerProfileService;
    private GameConfig _gameConfig;
    private Action _onSubmitted;
    private string _nickname = string.Empty;
    private string _error = string.Empty;
    private bool _isVisible;

    [Inject]
    public void Construct(PlayerProfileService playerProfileService, GameConfig gameConfig)
    {
        _playerProfileService = playerProfileService;
        _gameConfig = gameConfig;
    }

    public void Show(Action onSubmitted)
    {
        _onSubmitted = onSubmitted;
        _error = string.Empty;

        if (_gameConfig.UseTestNickname && _playerProfileService.TrySetNickname(_gameConfig.TestNickname))
        {
            Hide();
            _onSubmitted?.Invoke();
            return;
        }

        _nickname = _gameConfig.UseTestNickname ? _gameConfig.TestNickname : string.Empty;
        _error = _gameConfig.UseTestNickname ? "Test nickname cannot be empty." : string.Empty;
        _isVisible = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _isVisible = false;
        gameObject.SetActive(false);
    }

    private void OnGUI()
    {
        if (!_isVisible)
            return;

        Rect windowRect = new Rect(
            (Screen.width - WindowWidth) * 0.5f,
            (Screen.height - WindowHeight) * 0.5f,
            WindowWidth,
            WindowHeight);

        GUI.ModalWindow(GetInstanceID(), windowRect, DrawWindow, "Enter nickname");
    }

    private void DrawWindow(int windowId)
    {
        GUILayout.Space(10);
        GUILayout.Label("Nickname for leaderboard:");
        GUI.SetNextControlName("NicknameInput");
        _nickname = GUILayout.TextField(_nickname, 24);

        if (!string.IsNullOrEmpty(_error))
            GUILayout.Label(_error);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Continue") || IsSubmitPressed())
            SubmitNickname(_nickname);

        GUI.FocusControl("NicknameInput");
    }

    private bool IsSubmitPressed()
    {
        Event currentEvent = Event.current;
        return currentEvent != null
            && currentEvent.type == EventType.KeyDown
            && (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter);
    }

    private void SubmitNickname(string nickname)
    {
        if (!_playerProfileService.TrySetNickname(nickname))
        {
            _error = "Nickname cannot be empty.";
            return;
        }

        Hide();
        _onSubmitted?.Invoke();
    }
}
