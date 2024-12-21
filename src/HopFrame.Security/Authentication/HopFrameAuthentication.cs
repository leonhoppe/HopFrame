using System.Security.Claims;
using System.Text.Encodings.Web;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
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
    IOptions<HopFrameAuthenticationOptions> tokenOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock) {

    public const string SchemeName = "HopFrame.Authentication";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var accessToken = Request.Cookies[ITokenContext.AccessTokenType];
        if (string.IsNullOrEmpty(accessToken)) accessToken = Request.Headers[SchemeName];
        if (string.IsNullOrEmpty(accessToken)) accessToken = Request.Headers["Token"];
        if (string.IsNullOrEmpty(accessToken)) accessToken = Request.Query["token"];
        if (string.IsNullOrEmpty(accessToken)) return AuthenticateResult.Fail("No Access Token provided");

        var tokenEntry = await tokens.GetToken(accessToken);
        
        if (tokenEntry is null) return AuthenticateResult.Fail("The provided Access Token does not exist");

        if (tokenEntry.Type == Token.ApiTokenType) {
            if (tokenEntry.CreatedAt < DateTime.Now) return AuthenticateResult.Fail("The provided API Token is expired");
        }else if (tokenEntry.CreatedAt + tokenOptions.Value.AccessTokenTime < DateTime.Now) return AuthenticateResult.Fail("The provided Access Token is expired");
        
        if (tokenEntry.Owner is null)
            return AuthenticateResult.Fail("The provided Access Token does not match any user");

        var claims = new List<Claim> {
            new(HopFrameClaimTypes.AccessTokenId, accessToken),
            new(HopFrameClaimTypes.UserId, tokenEntry.Owner.Id.ToString())
        };

        var permissions  = await perms.GetFullPermissions(tokenEntry);
        claims.AddRange(permissions.Select(perm => new Claim(HopFrameClaimTypes.Permission, perm)));

        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity(claims, SchemeName));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
    
}