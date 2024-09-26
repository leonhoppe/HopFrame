using System.Security.Claims;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Web.Services;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Web;

public sealed class AuthMiddleware(IAuthService auth, IPermissionRepository perms) : IMiddleware {
    public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
        var loggedIn = await auth.IsLoggedIn();

        if (!loggedIn) {
            var token = await auth.RefreshLogin();
            if (token is null) {
                await next.Invoke(context);
                return;
            }
            
            var claims = new List<Claim> {
                new(HopFrameClaimTypes.AccessTokenId, token.Content.ToString()),
                new(HopFrameClaimTypes.UserId, token.Owner.Id.ToString())
            };

            var permissions = await perms.GetFullPermissions(token.Owner);
            claims.AddRange(permissions.Select(perm => new Claim(HopFrameClaimTypes.Permission, perm)));
            
            context.User.AddIdentity(new ClaimsIdentity(claims, HopFrameAuthentication.SchemeName));
        }
        
        await next?.Invoke(context);
    }
}