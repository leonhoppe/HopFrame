using System.Security.Claims;
using System.Text.Encodings.Web;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authentication.OpenID.Options;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS0618 // Type or member is obsolete

namespace HopFrame.Security.Authentication;

public class HopFrameAuthentication(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    ITokenRepository tokens,
    IPermissionRepository perms,
    IOptions<HopFrameAuthenticationOptions> tokenOptions,
    IOptions<OpenIdOptions> openIdOptions,
    IUserRepository users,
    IOpenIdAccessor accessor)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock) {

    public const string SchemeName = "HopFrame.Authentication";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var accessToken = Request.Cookies[ITokenContext.AccessTokenType];
        if (string.IsNullOrEmpty(accessToken)) accessToken = Request.Headers[SchemeName];
        if (string.IsNullOrEmpty(accessToken)) accessToken = Request.Headers["Token"];
        if (string.IsNullOrEmpty(accessToken)) accessToken = Request.Query["token"];
        if (string.IsNullOrEmpty(accessToken)) return AuthenticateResult.Fail("No Access Token provided");
        
        var tokenEntry = await tokens.GetToken(accessToken);

        if (tokenEntry?.Type != Token.ApiTokenType && openIdOptions.Value.Enabled) {
            var result = await accessor.InspectToken(accessToken);

            if (result is null || !result.Active)
                    return AuthenticateResult.Fail("Invalid OpenID Connect token");
            
            var email = result.Email;
            if (string.IsNullOrEmpty(email))
                return AuthenticateResult.Fail("OpenID user has no email associated to it");
    
            var user = await users.GetUserByEmail(email);
            if (user is null) {
                if (!openIdOptions.Value.GenerateUsers)
                    return AuthenticateResult.Fail("OpenID user does not exist");

                var username = result.PreferredUsername;
                user = await users.AddUser(new User {
                    Email = email,
                    Username = username
                });
            }
    
            var token = new Token {
                Owner = user,
                CreatedAt = DateTime.Now,
                Type = Token.OpenIdTokenType
            };
            var identity = await GenerateClaims(token);
            return AuthenticateResult.Success(new AuthenticationTicket(identity, Scheme.Name));
        }
        
        if (tokenEntry is null) return AuthenticateResult.Fail("The provided Access Token does not exist");

        if (tokenEntry.Type == Token.ApiTokenType) {
            if (tokenEntry.CreatedAt < DateTime.Now) return AuthenticateResult.Fail("The provided API Token is expired");
        }else if (tokenEntry.CreatedAt + tokenOptions.Value.AccessTokenTime < DateTime.Now) return AuthenticateResult.Fail("The provided Access Token is expired");
        
        if (tokenEntry.Owner is null)
            return AuthenticateResult.Fail("The provided Access Token does not match any user");

        var principal = await GenerateClaims(tokenEntry);
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }

    private async Task<ClaimsPrincipal> GenerateClaims(Token token) {
        var claims = new List<Claim> {
            new(HopFrameClaimTypes.AccessTokenId, token.TokenId.ToString()),
            new(HopFrameClaimTypes.UserId, token.Owner.Id.ToString())
        };

        var permissions  = await perms.GetFullPermissions(token);
        claims.AddRange(permissions.Select(perm => new Claim(HopFrameClaimTypes.Permission, perm)));

        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity(claims, SchemeName));
        return principal;
    }
    
}