using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Security.Claims;

internal sealed class TokenContextImplementor(IHttpContextAccessor accessor, IUserRepository users, ITokenRepository tokens) : ITokenContext {
    public bool IsAuthenticated => !string.IsNullOrEmpty(accessor.HttpContext?.User.GetAccessTokenId());

    public User User => users.GetUser(Guid.Parse(accessor.HttpContext?.User.GetUserId() ?? Guid.Empty.ToString())).GetAwaiter().GetResult();

    public Token AccessToken => tokens.GetToken(accessor.HttpContext?.User.GetAccessTokenId()).GetAwaiter().GetResult();
}