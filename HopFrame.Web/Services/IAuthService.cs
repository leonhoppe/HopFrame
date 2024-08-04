using HopFrame.Database.Models.Entries;
using HopFrame.Security.Models;

namespace HopFrame.Web.Services;

public interface IAuthService {
    Task Register(UserRegister register);
    Task<bool> Login(UserLogin login);
    Task Logout();

    Task<TokenEntry> RefreshLogin();
    Task<bool> IsLoggedIn();
}