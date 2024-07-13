using HopFrame.Database;
using HopFrame.Database.Models;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Security.Claims;

internal class TokenContextImplementor<TDbContext>(IHttpContextAccessor accessor, TDbContext context) : ITokenContext where TDbContext : HopDbContextBase {
    public bool IsAuthenticated => accessor.HttpContext?.User.Identity?.IsAuthenticated == true;

    public User User => context.Users
        .SingleOrDefault(user => user.Id == accessor.HttpContext.User.GetUserId())?
        .ToUserModel(context);
    
    public Guid AccessToken => Guid.Parse(accessor.HttpContext?.User.GetAccessTokenId() ?? string.Empty);
}