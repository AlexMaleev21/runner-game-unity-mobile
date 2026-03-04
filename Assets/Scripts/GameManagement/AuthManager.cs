using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class AuthManager : IInitializable
{
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
        Debug.Log("Auth");
        _authWindow.Hide();
    }

    public async Task<bool> AutoLogin()
    {
        return await _authService.AutoLogin();
    }

    public void ShowAuth()
    {
        Debug.Log(_authWindow);
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
    }
}
