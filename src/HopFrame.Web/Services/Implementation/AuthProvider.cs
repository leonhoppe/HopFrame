using HopFrame.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Web.Services.Implementation;

internal sealed class AuthProvider(IHttpContextAccessor accessor, HopFrameConfig config) : IAuthProvider {
    
    public Task<bool> IsAuthenticated(string? claim, CancellationToken cancellationToken) {
        if (config.AllowAnonymousAccess)
            return Task.FromResult(true);

        var context = accessor.HttpContext;
        if (context?.User.Identity is null)
            return Task.FromResult(false);

        if (string.IsNullOrWhiteSpace(claim))
            return Task.FromResult(true);

        return Task.FromResult(context.User.HasClaim(config.ClaimType, claim));
    }
    
}