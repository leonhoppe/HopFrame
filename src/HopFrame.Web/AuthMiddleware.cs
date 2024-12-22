using System.Security.Claims;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Web.Services;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Web;

/// <summary>
/// Assures that the user stays logged in even if the access token is expired
/// </summary>
public sealed class AuthMiddleware(IAuthService auth, IPermissionRepository perms) : IMiddleware {
    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        var loggedIn = await auth.IsLoggedIn();

        if (!loggedIn) {
            var token = await auth.RefreshLogin();
            if (token is null) {
                next?.Invoke(context);
                return;
            }

            var principal = await HopFrameAuthentication.GenerateClaims(token, perms);
            if (principal?.Identity is ClaimsIdentity identity)
                context.User.AddIdentity(identity);
        }
        
        await next?.Invoke(context);
    }
}