using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication.OpenID.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HopFrame.Security.Claims;

internal sealed class TokenContextImplementor(IHttpContextAccessor accessor, IUserRepository users, ITokenRepository tokens, IOptions<OpenIdOptions> options) : ITokenContext {
    public bool IsAuthenticated => !string.IsNullOrEmpty(accessor.HttpContext?.User.GetAccessTokenId());

    public User User => users.GetUser(Guid.Parse(accessor.HttpContext?.User.GetUserId() ?? Guid.Empty.ToString())).GetAwaiter().GetResult();

    public Token AccessToken => options.Value.Enabled ? new Token {
        Owner = User,
        Type = Token.OpenIdTokenType,
        CreatedAt = DateTime.Now
    } : tokens.GetToken(accessor.HttpContext?.User.GetAccessTokenId()).GetAwaiter().GetResult();
}