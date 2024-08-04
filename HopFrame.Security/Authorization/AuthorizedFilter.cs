using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HopFrame.Security.Authorization;

public class AuthorizedFilter : IAuthorizationFilter {
    private readonly string[] _permissions;

    public AuthorizedFilter(params string[] permissions) {
        _permissions = permissions;
    }
    
    public void OnAuthorization(AuthorizationFilterContext context) {
        if (context.Filters.Any(item => item is IAllowAnonymousFilter)) return;

        if (string.IsNullOrEmpty(context.HttpContext.User.GetAccessTokenId())) {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        if (_permissions.Length == 0) return;

        var permissions = context.HttpContext.User.GetPermissions();

        if (!_permissions.All(permission => PermissionValidator.IncludesPermission(permission, permissions))) {
            context.Result = new UnauthorizedResult();
            return;
        }
    }
}