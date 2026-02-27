using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System;
using TMPro;

public class AuthWindow : BaseWindow
{
    [Header("Login Panel")]
    [SerializeField] private GameObject _loginPanel;
    [SerializeField] private TMP_InputField _loginEmailInput;
    [SerializeField] private TMP_InputField _loginPasswordInput;
    [SerializeField] private Button _loginButton;
    [SerializeField] private Button _switchToRegisterButton;

    [Header("Register Panel")]
    [SerializeField] private GameObject _registerPanel;
    [SerializeField] private TMP_InputField _registerEmailInput;
    [SerializeField] private TMP_InputField _usernameInput;
    [SerializeField] private TMP_InputField _registerPasswordInput;
    [SerializeField] private TMP_InputField _confirmPasswordInput;
    [SerializeField] private Button _registerButton;
    [SerializeField] private Button _switchToLoginButton;

    [Header("Common")]
    [SerializeField] private TextMeshProUGUI _errorText;

    private IAuthService _authService;
    private SignalBus _signalBus;

    [Inject]
    public void Construct(IAuthService authService, SignalBus signalBus)
    {
        _authService = authService;
        _signalBus = signalBus;
    }

    private void Start()
    {
        _loginButton.onClick.AddListener(OnLoginClicked);
        _registerButton.onClick.AddListener(OnRegisterClicked);
        _switchToRegisterButton.onClick.AddListener(() => ShowRegistrationPanel(true));
        _switchToLoginButton.onClick.AddListener(() => ShowRegistrationPanel(false));

        _loginPanel.SetActive(false);
        _registerPanel.SetActive(false);

        ShowRegistrationPanel(false);
        ClearError();
    }

    private void ShowRegistrationPanel(bool isRegister)
    {
        ClearFields();
        _loginPanel.SetActive(!isRegister);
        _registerPanel.SetActive(isRegister);
        ClearError();
    }

    private void ClearError()
    {
        _errorText.text = "";
    }

    public void ClearFields()
    {
        _loginEmailInput.text = string.Empty;
        _loginPasswordInput.text = string.Empty;
        _registerEmailInput.text = string.Empty;
        _usernameInput.text = string.Empty;
        _registerPasswordInput.text = string.Empty;
        _confirmPasswordInput.text = string.Empty;
    }
    private async void OnLoginClicked()
    {
        string email = _loginEmailInput.text;
        string password = _loginPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            _errorText.text = "Email and password required";
            return;
        }

        _loginButton.interactable = false;
        bool success = await _authService.Login(email, password);
        _loginButton.interactable = true;

        if (success)
        {
            _signalBus.Fire(new AuthSuccessSignal());
            Hide();
        }
        else
        {
            _errorText.text = "Invalid email or password";
        }
    }

    private async void OnRegisterClicked()
    {
        string email = _registerEmailInput.text;
        string username = _usernameInput.text;
        string password = _registerPasswordInput.text;
        string confirm = _confirmPasswordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            _errorText.text = "All fields required";
            return;
        }

        if (password != confirm)
        {
            _errorText.text = "Passwords do not match";
            return;
        }

        _registerButton.interactable = false;
        bool success = await _authService.Register(email, password, username);
        _registerButton.interactable = true;

        if (success)
        {
            _signalBus.Fire(new AuthSuccessSignal());
            Hide();
        }
        else
        {
            _errorText.text = "Registration failed. Email may already be in use.";
        }
    }
}

public struct AuthSuccessSignal { }
