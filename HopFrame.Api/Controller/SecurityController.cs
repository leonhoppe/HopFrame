using System.Globalization;
using System.Text;
using HopFrame.Api.Logic;
using HopFrame.Api.Models;
using HopFrame.Database;
using HopFrame.Database.Models.Entries;
using HopFrame.Security.Authentication;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Api.Controller;

[ApiController]
[Route("authentication")]
public class SecurityController<TDbContext>(TDbContext context) : ControllerBase where TDbContext : HopDbContextBase {

    private const string RefreshTokenType = "HopFrame.Security.RefreshToken";

    [HttpPut("login")]
    public async Task<ActionResult<SingleValueResult<string>>> Login([FromBody] UserLogin login) {
        var user = await context.Users.SingleOrDefaultAsync(user => user.Email == login.Email);

        if (user is null)
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.NotFound("The provided email address was not found"));

        var hashedPassword = EncryptionManager.Hash(login.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        if (hashedPassword != user.Password)
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Forbidden("The provided password is not correct"));

        var refreshToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.RefreshTokenType,
            UserId = user.Id
        };
        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = user.Id
        };
        
        HttpContext.Response.Cookies.Append(RefreshTokenType, refreshToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });

        await context.Tokens.AddRangeAsync(refreshToken, accessToken);
        await context.SaveChangesAsync();

        return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Ok(accessToken.Token));
    }

    [HttpPost("register")]
    public async Task<ActionResult<SingleValueResult<string>>> Register([FromBody] UserRegister register) {
        if (register.Password.Length < 8)
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Conflict("Password needs to be at least 8 characters long"));
        
        if (await context.Users.AnyAsync(user => user.Username == register.Username || user.Email == register.Email))
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Conflict("Username or Email is already registered"));

        var user = new UserEntry {
            CreatedAt = DateTime.Now,
            Email = register.Email,
            Username = register.Username,
            Id = Guid.NewGuid().ToString()
        };
        user.Password = EncryptionManager.Hash(register.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));

        await context.Users.AddAsync(user);

        var defaultGroups = await context.Groups
            .Where(group => group.Default)
            .Select(group => "group." + group.Name)
            .ToListAsync();

        await context.Permissions.AddRangeAsync(defaultGroups.Select(group => new PermissionEntry {
            GrantedAt = DateTime.Now,
            PermissionText = group,
            UserId = user.Id
        }));
        
        var refreshToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.RefreshTokenType,
            UserId = user.Id
        };
        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = user.Id
        };
        
        HttpContext.Response.Cookies.Append(RefreshTokenType, refreshToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });

        await context.Tokens.AddRangeAsync(refreshToken, accessToken);
        await context.SaveChangesAsync();

        return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Ok(accessToken.Token));
    }

    [HttpGet("authenticate")]
    public async Task<ActionResult<SingleValueResult<string>>> Authenticate() {
        var refreshToken = HttpContext.Request.Cookies[RefreshTokenType];
        
        if (string.IsNullOrEmpty(refreshToken))
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Conflict("Refresh token not provided"));

        var token = await context.Tokens.SingleOrDefaultAsync(token => token.Token == refreshToken && token.Type == TokenEntry.RefreshTokenType);
        
        if (token is null)
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.NotFound("Refresh token not valid"));

        if (token.CreatedAt + HopFrameAuthentication<TDbContext>.RefreshTokenTime < DateTime.Now)
            return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Conflict("Refresh token is expired"));

        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = token.UserId
        };

        await context.Tokens.AddAsync(accessToken);
        await context.SaveChangesAsync();
        
        return this.FromLogicResult(LogicResult<SingleValueResult<string>>.Ok(accessToken.Token));
    }

    [HttpDelete("logout"), Authorized]
    public async Task<ActionResult> Logout() {
        var accessToken = HttpContext.User.GetAccessTokenId();
        var refreshToken = HttpContext.Request.Cookies[RefreshTokenType];
        
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return this.FromLogicResult(LogicResult.Conflict("access or refresh token not provided"));

        var tokenEntries = await context.Tokens.Where(token =>
                (token.Token == accessToken && token.Type == TokenEntry.AccessTokenType) ||
                (token.Token == refreshToken && token.Type == TokenEntry.RefreshTokenType))
            .ToArrayAsync();
        
        if (tokenEntries.Length != 2)
            return this.FromLogicResult(LogicResult.NotFound("One or more of the provided tokens was not found"));

        context.Tokens.Remove(tokenEntries[0]);
        context.Tokens.Remove(tokenEntries[1]);
        await context.SaveChangesAsync();
        
        HttpContext.Response.Cookies.Delete(RefreshTokenType);
        
        return this.FromLogicResult(LogicResult.Ok());
    }

    [HttpDelete("delete"), Authorized]
    public async Task<ActionResult> Delete([FromBody] UserLogin login) {
        var token = HttpContext.User.GetAccessTokenId();
        var userId = (await context.Tokens.SingleOrDefaultAsync(t => t.Token == token && t.Type == TokenEntry.AccessTokenType))?.UserId;

        if (string.IsNullOrEmpty(userId))
            return this.FromLogicResult(LogicResult.NotFound("Access token does not match any user"));

        var user = await context.Users.SingleAsync(user => user.Id == userId);
        
        var password = EncryptionManager.Hash(login.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        if (user.Password != password)
            return this.FromLogicResult(LogicResult.Forbidden("The provided password is not correct"));

        var tokens = await context.Tokens.Where(t => t.UserId == userId).ToArrayAsync();
        var permissions = await context.Permissions.Where(perm => perm.UserId == userId).ToArrayAsync();
        
        context.Tokens.RemoveRange(tokens);
        context.Permissions.RemoveRange(permissions);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        
        HttpContext.Response.Cookies.Delete(RefreshTokenType);

        return this.FromLogicResult(LogicResult.Ok());
    }
    
}