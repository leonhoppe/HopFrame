using System.Globalization;
using System.Text;
using HopFrame.Api.Logic;
using HopFrame.Api.Models;
using HopFrame.Database;
using HopFrame.Database.Models.Entries;
using HopFrame.Security;
using HopFrame.Security.Authentication;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using HopFrame.Security.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Api.Controller;

[ApiController]
[Route("authentication")]
public class SecurityController<TDbContext>(TDbContext context, IUserService users, ITokenContext tokenContext) : ControllerBase where TDbContext : HopDbContextBase {

    private const string RefreshTokenType = "HopFrame.Security.RefreshToken";

    [HttpPut("login")]
    public async Task<ActionResult<SingleValueResult<string>>> Login([FromBody] UserLogin login) {
        var user = await users.GetUserByEmail(login.Email);

        if (user is null)
            return LogicResult<SingleValueResult<string>>.NotFound("The provided email address was not found");

        var hashedPassword = EncryptionManager.Hash(login.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        if (hashedPassword != await users.GetUserPassword(user))
            return LogicResult<SingleValueResult<string>>.Forbidden("The provided password is not correct");

        var refreshToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.RefreshTokenType,
            UserId = user.Id.ToString()
        };
        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = user.Id.ToString()
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
    public async Task<ActionResult<SingleValueResult<string>>> Register([FromBody] UserRegister register) {
        if (register.Password.Length < 8)
            return LogicResult<SingleValueResult<string>>.Conflict("Password needs to be at least 8 characters long");

        var allUsers = await users.GetUsers();
        if (allUsers.Any(user => user.Username == register.Username || user.Email == register.Email))
            return LogicResult<SingleValueResult<string>>.Conflict("Username or Email is already registered");

        var user = await users.AddUser(register);
        
        var refreshToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.RefreshTokenType,
            UserId = user.Id.ToString()
        };
        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = user.Id.ToString()
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
    public async Task<ActionResult<SingleValueResult<string>>> Authenticate() {
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
    public async Task<ActionResult> Logout() {
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
    public async Task<ActionResult> Delete([FromBody] UserPasswordValidation validation) {
        var user = tokenContext.User;
        
        var password = EncryptionManager.Hash(validation.Password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        if (await users.GetUserPassword(user) != password)
            return LogicResult.Forbidden("The provided password is not correct");

        await users.DeleteUser(user);
        
        HttpContext.Response.Cookies.Delete(RefreshTokenType);

        return LogicResult.Ok();
    }
    
}