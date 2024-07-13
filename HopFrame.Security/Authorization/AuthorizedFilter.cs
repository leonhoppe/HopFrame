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

        if (context.HttpContext.User.Identity?.IsAuthenticated == false) {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        //TODO: Check Permissions
    }
}