using System.Security.Claims;
using System.Text.Encodings.Web;
using HopFrame.Database;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable CS0618 // Type or member is obsolete

namespace HopFrame.Security.Authentication;

public class HopFrameAuthentication<TDbContext> : AuthenticationHandler<AuthenticationSchemeOptions> where TDbContext : HopDbContextBase {

    public const string SchemeName = "HopCore.Authentication";
    public static readonly TimeSpan AccessTokenTime = new(0, 0, 5, 0);
    public static readonly TimeSpan RefreshTokenTime = new(30, 0, 0, 0);

    private readonly TDbContext _context;

    public HopFrameAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, TDbContext context) : base(options, logger, encoder, clock) {
        _context = context;
    }
    
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
        var accessToken = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(accessToken)) return AuthenticateResult.Fail("No Access Token provided");
        
        var tokenEntry = await _context.Tokens.SingleOrDefaultAsync(token => token.Token == accessToken);
        
        if (tokenEntry is null) return AuthenticateResult.Fail("The provided Access Token does not exist");
        if (tokenEntry.CreatedAt + AccessTokenTime < DateTime.Now) return AuthenticateResult.Fail("The provided Access Token is expired");
        
        if (!(await _context.Users.AnyAsync(user => user.Id == tokenEntry.UserId)))
            return AuthenticateResult.Fail("The provided Access Token does not match any user");

        var claims = new List<Claim> {
            new(HopFrameClaimTypes.AccessTokenId, accessToken),
            new(HopFrameClaimTypes.UserId, tokenEntry.UserId)
        };

        var permissions = await _context.Permissions
            .Where(perm => perm.UserId == tokenEntry.UserId)
            .Select(perm => perm.PermissionText)
            .ToListAsync();

        var groups = permissions
            .Where(perm => perm.StartsWith("group."))
            .ToList();

        var groupPerms = await _context.Permissions
            .Where(perm => groups.Contains(perm.UserId))
            .Select(perm => perm.PermissionText)
            .ToListAsync();
        
        permissions.AddRange(groupPerms);
        
        claims.AddRange(permissions.Select(perm => new Claim(HopFrameClaimTypes.Permission, perm)));

        var principal = new ClaimsPrincipal();
        principal.AddIdentity(new ClaimsIdentity(claims, SchemeName));
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
    
}