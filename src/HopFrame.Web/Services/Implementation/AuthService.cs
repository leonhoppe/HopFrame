using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Web.Services.Implementation;

internal class AuthService(
    IUserRepository userService,
    IHttpContextAccessor httpAccessor,
    ITokenRepository tokens,
    ITokenContext context)
    : IAuthService {
    
    public async Task Register(UserRegister register) {
        var user = await userService.AddUser(new User {
            Username = register.Username,
            Email = register.Email,
            Password = register.Password
        });
        
        if (user is null) return;

        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });
    }

    public async Task<bool> Login(UserLogin login) {
        var user = await userService.GetUserByEmail(login.Email);

        if (user == null) return false;
        if (await userService.CheckUserPassword(user, login.Password) == false) return false;
        
        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return true;
    }

    public async Task Logout() {
        await tokens.DeleteUserTokens(context.User);
        
        httpAccessor.HttpContext?.Response.Cookies.Delete(ITokenContext.RefreshTokenType);
        httpAccessor.HttpContext?.Response.Cookies.Delete(ITokenContext.AccessTokenType);
    }

    public async Task<Token> RefreshLogin() {
        var refreshToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];

        if (string.IsNullOrWhiteSpace(refreshToken)) return null;

        var token = await tokens.GetToken(refreshToken);

        if (token is null || token.Type != Token.RefreshTokenType) return null;
        
        if (token.CreatedAt + HopFrameAuthentication.RefreshTokenTime < DateTime.Now) return null;

        var accessToken = await tokens.CreateToken(Token.AccessTokenType, token.Owner);
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return accessToken;
    }

    public async Task<bool> IsLoggedIn() {
        var accessToken = httpAccessor.HttpContext?.Request.Cookies[ITokenContext.AccessTokenType];
        if (string.IsNullOrEmpty(accessToken)) return false;

        var tokenEntry = await tokens.GetToken(accessToken);
        
        if (tokenEntry is null) return false;
        if (tokenEntry.Type != Token.AccessTokenType) return false;
        if (tokenEntry.CreatedAt + HopFrameAuthentication.AccessTokenTime < DateTime.Now) return false;
        if (tokenEntry.Owner is null) return false;

        return true;
    }
}