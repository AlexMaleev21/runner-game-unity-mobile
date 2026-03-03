using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class FirebaseAuthService : IAuthService
{
    private FirebaseAuth _auth;
    private FirebaseUser _user;

    public bool IsAuthenticated => _user != null;
    public string UserId => _user.UserId;
    public string UserEmail => _user.Email;
    public string UserName => _user.DisplayName;

    public FirebaseAuthService()
    {
        InitializeFirebase();
    }

    private async void InitializeFirebase()
    {
        //проверка файлов SDK (напоминание)
        await FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                _auth = FirebaseAuth.DefaultInstance;
                _auth.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
            }
            else
            {
                Debug.LogError($"Dependencies problem {dependencyStatus}");
            }
        });
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
        if (_auth.CurrentUser != _user)
        {
            _user = _auth.CurrentUser;
        }
    }

    public async Task<bool> Register(string email, string password, string username)
    {
        try
        {
            var result = await _auth.CreateUserWithEmailAndPasswordAsync(email, password);
            var profile = new UserProfile { DisplayName = username };
            await result.User.UpdateUserProfileAsync(profile);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Registration failed {e.Message}");
            return false;
        }

    }

    public async Task<bool> Login(string email, string password)
    {

        try
        {
            await _auth.SignInWithEmailAndPasswordAsync(email, password);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Login failed {e.Message}");
            return false;
        }

    }

    public async Task<bool> AutoLogin()
    {
        float timeout = 3f;
        float startTime = Time.time;

        while (_auth == null && Time.time - startTime < timeout)
        {
            await Task.Delay(50);
        }

        return _auth.CurrentUser != null;
    }

    public void Logout()
    {
        _auth?.SignOut();
    }
}