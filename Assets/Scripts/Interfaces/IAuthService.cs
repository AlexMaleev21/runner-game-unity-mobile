using System;
using System.Threading.Tasks;

public interface IAuthService
{
    bool IsAuthenticated { get; }
    string UserId { get; }
    string UserEmail { get; }

    Task<bool> Register(string email, string password, string username);
    Task<bool> Login(string email, string password);
    Task<bool> AutoLogin();
    void Logout();
}
