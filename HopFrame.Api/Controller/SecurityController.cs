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
    public async Task<ILogicResult<SingleValueResult<string>>> Login([FromBody] UserLogin login) {
        var user = await context.Users.SingleOrDefaultAsync(user => user.Email == login.Email);

        if (user is null)
            return LogicResult<SingleValueResult<string>>.NotFound("The provided email address was not found");

        var hashedPassword = EncryptionManager.Hash(login.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        if (hashedPassword != user.Password)
            return LogicResult<SingleValueResult<string>>.Forbidden("The provided password is not correct");

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

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Token);
    }

    [HttpPost("register")]
    public async Task<ILogicResult<SingleValueResult<string>>> Register([FromBody] UserRegister register) {
        //TODO: Validate Password requirements
        
        if (await context.Users.AnyAsync(user => user.Username == register.Username || user.Email == register.Email))
            return LogicResult<SingleValueResult<string>>.Conflict("Username or Email is already registered");

        var user = new UserEntry {
            CreatedAt = DateTime.Now,
            Email = register.Email,
            Username = register.Username,
            Id = Guid.NewGuid().ToString()
        };
        user.Password = EncryptionManager.Hash(register.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));

        await context.Users.AddAsync(user);
        
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

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Token);
    }

    [HttpGet("authenticate")]
    public async Task<ILogicResult<SingleValueResult<string>>> Authenticate() {
        var refreshToken = HttpContext.Request.Cookies[RefreshTokenType];
        
        if (string.IsNullOrEmpty(refreshToken))
            return LogicResult<SingleValueResult<string>>.Conflict("Refresh token not provided");

        var token = await context.Tokens.SingleOrDefaultAsync(token => token.Token == refreshToken && token.Type == TokenEntry.RefreshTokenType);
        
        if (token is null)
            return LogicResult<SingleValueResult<string>>.NotFound("Refresh token not valid");

        if (token.CreatedAt + HopFrameAuthentication<TDbContext>.RefreshTokenTime < DateTime.Now)
            return LogicResult<SingleValueResult<string>>.Conflict("Refresh token is expired");

        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = token.UserId
        };

        await context.Tokens.AddAsync(accessToken);
        await context.SaveChangesAsync();
        
        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Token);
    }

    [HttpDelete("logout"), Authorized]
    public async Task<ILogicResult> Logout() {
        var accessToken = HttpContext.User.GetAccessTokenId();
        var refreshToken = HttpContext.Request.Cookies[RefreshTokenType];
        
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return LogicResult.Conflict("access or refresh token not provided");

        var tokenEntries = await context.Tokens.Where(token =>
                (token.Token == accessToken && token.Type == TokenEntry.AccessTokenType) ||
                (token.Token == refreshToken && token.Type == TokenEntry.RefreshTokenType))
            .ToArrayAsync();
        
        if (tokenEntries.Length != 2)
            return LogicResult.NotFound("One or more of the provided tokens was not found");

        context.Tokens.Remove(tokenEntries[0]);
        context.Tokens.Remove(tokenEntries[1]);
        await context.SaveChangesAsync();
        
        HttpContext.Response.Cookies.Delete(RefreshTokenType);
        
        return LogicResult.Ok();
    }

    [HttpDelete("delete"), Authorized]
    public async Task<ILogicResult> Delete([FromBody] UserLogin login) {
        var token = HttpContext.User.GetAccessTokenId();
        var userId = (await context.Tokens.SingleOrDefaultAsync(t => t.Token == token && t.Type == TokenEntry.AccessTokenType))?.UserId;

        if (string.IsNullOrEmpty(userId))
            return LogicResult.NotFound("Access token does not match any user");

        var user = await context.Users.SingleAsync(user => user.Id == userId);
        
        var password = EncryptionManager.Hash(login.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        if (user.Password != password)
            return LogicResult.Forbidden("The provided password is not correct");

        var tokens = await context.Tokens.Where(t => t.UserId == userId).ToArrayAsync();
        
        context.Tokens.RemoveRange(tokens);
        context.Users.Remove(user);
        await context.SaveChangesAsync();
        
        HttpContext.Response.Cookies.Delete(RefreshTokenType);

        return LogicResult.Ok();
    }
    
}