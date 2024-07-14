using HopFrame.Database;
using HopFrame.Database.Models.Entries;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using HopFrame.Security.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Web.Services.Implementation;

public class AuthService<TDbContext>(
    IUserService userService,
    IHttpContextAccessor httpAccessor,
    TDbContext context)
    : IAuthService where TDbContext : HopDbContextBase {
    
    public async Task Register(UserRegister register) {
        var user = await userService.AddUser(register);
        if (user is null) return;
        
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
        
        context.Tokens.AddRange(refreshToken, accessToken);
        await context.SaveChangesAsync();
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<HopDbContextBase>.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });
    }

    public async Task<bool> Login(UserLogin login) {
        var user = await userService.GetUserByEmail(login.Email);

        if (user == null) return false;
        if (await userService.CheckUserPassword(user, login.Password) == false) return false;
        
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
        
        context.Tokens.AddRange(refreshToken, accessToken);
        await context.SaveChangesAsync();
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<HopDbContextBase>.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return true;
    }

    public async Task Logout() {
        var accessToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.AccessTokenType];
        var refreshToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
        var tokenEntries = await context.Tokens.Where(token =>
                (token.Token == accessToken && token.Type == TokenEntry.AccessTokenType) ||
                (token.Token == refreshToken && token.Type == TokenEntry.RefreshTokenType))
            .ToArrayAsync();
        
        context.Tokens.Remove(tokenEntries[0]);
        context.Tokens.Remove(tokenEntries[1]);
        await context.SaveChangesAsync();
        
        httpAccessor.HttpContext?.Response.Cookies.Delete(ITokenContext.RefreshTokenType);
        httpAccessor.HttpContext?.Response.Cookies.Delete(ITokenContext.AccessTokenType);
    }

    public async Task<TokenEntry> RefreshLogin() {
        if (await IsLoggedIn()) {
            var oldToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.AccessTokenType];
            var entry = await context.Tokens.SingleOrDefaultAsync(token => token.Token == oldToken);

            if (entry is not null) {
                context.Tokens.Remove(entry);
            }
        }
        
        var refreshToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];

        if (string.IsNullOrWhiteSpace(refreshToken)) return null;
        
        var token = await context.Tokens.SingleOrDefaultAsync(token => token.Token == refreshToken && token.Type == TokenEntry.RefreshTokenType);

        if (token is null) return null;
        if (token.CreatedAt + HopFrameAuthentication<TDbContext>.RefreshTokenTime < DateTime.Now) return null;
        
        var accessToken = new TokenEntry {
            CreatedAt = DateTime.Now,
            Token = Guid.NewGuid().ToString(),
            Type = TokenEntry.AccessTokenType,
            UserId = token.UserId
        };

        await context.Tokens.AddAsync(accessToken);
        await context.SaveChangesAsync();
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Token, new CookieOptions {
            MaxAge = HopFrameAuthentication<TDbContext>.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return accessToken;
    }

    public async Task<bool> IsLoggedIn() {
        var accessToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.AccessTokenType];
        if (string.IsNullOrEmpty(accessToken)) return false;
        
        var tokenEntry = await context.Tokens.SingleOrDefaultAsync(token => token.Token == accessToken);
        
        if (tokenEntry is null) return false;
        if (tokenEntry.CreatedAt + HopFrameAuthentication<TDbContext>.AccessTokenTime < DateTime.Now) return false;
        if (!await context.Users.AnyAsync(user => user.Id == tokenEntry.UserId)) return false;

        return true;
    }
}