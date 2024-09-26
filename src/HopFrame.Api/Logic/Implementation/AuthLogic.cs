using HopFrame.Api.Models;
using HopFrame.Database;
using HopFrame.Database.Models.Entries;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using HopFrame.Security.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Api.Logic.Implementation;

public class AuthLogic<TDbContext>(TDbContext context, IUserService users, ITokenContext tokenContext, IHttpContextAccessor accessor) : IAuthLogic where TDbContext : HopDbContextBase {
    
    public async Task<LogicResult<SingleValueResult<string>>> Login(UserLogin login) {
        var user = await users.GetUserByEmail(login.Email);

        if (user is null)
            return LogicResult<SingleValueResult<string>>.NotFound("The provided email address was not found");

        if (!await users.CheckUserPassword(user, login.Password))
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
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.AccessTokenTime,
            HttpOnly = true,
            Secure = true
        });

        await context.Tokens.AddRangeAsync(refreshToken, accessToken);
        await context.SaveChangesAsync();

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Token);
    }

    public async Task<LogicResult<SingleValueResult<string>>> Register(UserRegister register) {
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
        
        await context.Tokens.AddRangeAsync(refreshToken, accessToken);
        await context.SaveChangesAsync();
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Token);
    }

    public async Task<LogicResult<SingleValueResult<string>>> Authenticate() {
        var refreshToken = accessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
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
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });
        
        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Token);
    }

    public async Task<LogicResult> Logout() {
        var accessToken = accessor.HttpContext?.User.GetAccessTokenId();
        var refreshToken = accessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
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
        
        accessor.HttpContext?.Response.Cookies.Delete(ITokenContext.RefreshTokenType);
        accessor.HttpContext?.Response.Cookies.Delete(ITokenContext.AccessTokenType);
        
        return LogicResult.Ok();
    }

    public async Task<LogicResult> Delete(UserPasswordValidation validation) {
        var user = tokenContext.User;
        
        if (!await users.CheckUserPassword(user, validation.Password))
            return LogicResult.Forbidden("The provided password is not correct");

        await users.DeleteUser(user);
        
        accessor.HttpContext?.Response.Cookies.Delete(ITokenContext.RefreshTokenType);
        accessor.HttpContext?.Response.Cookies.Delete(ITokenContext.AccessTokenType);

        return LogicResult.Ok();
    }
    
}