using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authentication.OpenID.Options;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HopFrame.Web.Services.Implementation;

internal class AuthService(
    IUserRepository userService,
    IHttpContextAccessor httpAccessor,
    ITokenRepository tokens,
    ITokenContext context,
    IOptions<HopFrameAuthenticationOptions> options,
    IOptions<OpenIdOptions> openIdOptions,
    IOpenIdAccessor accessor,
    IUserRepository users)
    : IAuthService {
    
    public async Task Register(UserRegister register) {
        if (!options.Value.DefaultAuthentication) return;
        
        var user = await userService.AddUser(new User {
            Username = register.Username,
            Email = register.Email,
            Password = register.Password
        });
        
        if (user is null) return;

        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });
    }

    public async Task<bool> Login(UserLogin login) {
        if (!options.Value.DefaultAuthentication) return false;
        
        var user = await userService.GetUserByEmail(login.Email);

        if (user == null) return false;
        if (await userService.CheckUserPassword(user, login.Password) == false) return false;
        
        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.AccessTokenTime,
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

        if (openIdOptions.Value.Enabled && !Guid.TryParse(refreshToken, out _)) {
            var openIdToken = await accessor.RefreshAccessToken(refreshToken);

            if (openIdToken is null)
                return null;

            var inspection = await accessor.InspectToken(openIdToken.AccessToken);
            
            var email = inspection.Email;
            if (string.IsNullOrEmpty(email))
                return null;
    
            var user = await users.GetUserByEmail(email);
            if (user is null) {
                if (!openIdOptions.Value.GenerateUsers)
                    return null;

                var username = inspection.PreferredUsername;
                user = await users.AddUser(new User {
                    Email = email,
                    Username = username
                });
            }
            
            httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, openIdToken.AccessToken, new CookieOptions {
                MaxAge = TimeSpan.FromSeconds(openIdToken.ExpiresIn),
                HttpOnly = false,
                Secure = true
            });
            return new() {
                Owner = user,
                CreatedAt = DateTime.Now,
                Type = Token.OpenIdTokenType
            };
        }

        if (!options.Value.DefaultAuthentication)
            return null;

        var token = await tokens.GetToken(refreshToken);

        if (token is null || token.Type != Token.RefreshTokenType) return null;
        
        if (token.CreatedAt + options.Value.RefreshTokenTime < DateTime.Now) return null;

        var accessToken = await tokens.CreateToken(Token.AccessTokenType, token.Owner);
        
        httpAccessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return accessToken;
    }

    public async Task<bool> IsLoggedIn() {
        var accessToken = context.AccessToken;
        
        if (accessToken is null) return false;
        if (accessToken.Type != Token.AccessTokenType && accessToken.Type != Token.OpenIdTokenType) return false;
        if (accessToken.CreatedAt + options.Value.AccessTokenTime < DateTime.Now) return false;
        if (accessToken.Owner is null) return false;

        return true;
    }
}