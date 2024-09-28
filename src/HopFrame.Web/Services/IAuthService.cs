using HopFrame.Database.Models;
using HopFrame.Security.Models;

namespace HopFrame.Web.Services;

public interface IAuthService {
    Task Register(UserRegister register);
    Task<bool> Login(UserLogin login);
    Task Logout();

    Task<Token> RefreshLogin();
    Task<bool> IsLoggedIn();
}