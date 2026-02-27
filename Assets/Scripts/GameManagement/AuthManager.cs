using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class AuthManager : IInitializable
{
    public event Action OnLogoutRequested;
    public event Action OnAuthSuccess;

    private readonly IAuthService _authService;
    private AuthWindow _authWindow;

    public AuthManager(
        IAuthService authService,
        AuthWindow authWindow
        )
    {
        _authService = authService;
        _authWindow = authWindow;
    }

    public void Initialize()
    {
        _authWindow.Hide();
    }

    public async Task<bool> AutoLogin()
    {
        return await _authService.AutoLogin();
    }

    public void ShowAuth()
    {
        _authWindow.Show();
    }

    public void HideAuth()
    {
        if (_authWindow != null)
        {
            _authWindow.Hide();
            _authWindow = null;
        }
    }

    public void Logout()
    {
        _authService.Logout();
        OnLogoutRequested?.Invoke();
    }

    public void NotifyAuthSuccess()
    {
        OnAuthSuccess?.Invoke();
    }

}
