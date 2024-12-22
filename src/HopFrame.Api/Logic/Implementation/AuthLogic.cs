using HopFrame.Api.Models;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HopFrame.Api.Logic.Implementation;

internal class AuthLogic(IUserRepository users, ITokenRepository tokens, ITokenContext tokenContext, IHttpContextAccessor accessor, IOptions<HopFrameAuthenticationOptions> options) : IAuthLogic {
    
    public async Task<LogicResult<SingleValueResult<string>>> Login(UserLogin login) {
        if (!options.Value.DefaultAuthentication) return LogicResult<SingleValueResult<string>>.BadRequest("HopFrame authentication scheme is disabled");
        
        var user = await users.GetUserByEmail(login.Email);

        if (user is null)
            return LogicResult<SingleValueResult<string>>.NotFound("The provided email address was not found");

        if (!await users.CheckUserPassword(user, login.Password))
            return LogicResult<SingleValueResult<string>>.Forbidden("The provided password is not correct");

        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.AccessTokenTime,
            HttpOnly = true,
            Secure = true
        });

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.TokenId.ToString());
    }

    public async Task<LogicResult<SingleValueResult<string>>> Register(UserRegister register) {
        if (!options.Value.DefaultAuthentication) return LogicResult<SingleValueResult<string>>.BadRequest("HopFrame authentication scheme is disabled");
        
        if (register.Password.Length < 8)
            return LogicResult<SingleValueResult<string>>.BadRequest("Password needs to be at least 8 characters long");

        var allUsers = await users.GetUsers();
        if (allUsers.Any(user => user.Username == register.Username || user.Email == register.Email))
            return LogicResult<SingleValueResult<string>>.Conflict("Username or Email is already registered");

        var user = await users.AddUser(new User {
            Username = register.Username,
            Email = register.Email,
            Password = register.Password
        });
        
        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.TokenId.ToString());
    }

    public async Task<LogicResult<SingleValueResult<string>>> Authenticate() {
        if (!options.Value.DefaultAuthentication) return LogicResult<SingleValueResult<string>>.BadRequest("HopFrame authentication scheme is disabled");
        
        var refreshToken = accessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
        if (string.IsNullOrEmpty(refreshToken))
            return LogicResult<SingleValueResult<string>>.BadRequest("Refresh token not provided");

        var token = await tokens.GetToken(refreshToken);

        if (token is null)
            return LogicResult<SingleValueResult<string>>.NotFound("Refresh token not valid");

        if (token.Type != Token.RefreshTokenType)
            return LogicResult<SingleValueResult<string>>.Conflict("The provided token is not a refresh token");

        if (token.CreatedAt + options.Value.RefreshTokenTime < DateTime.Now)
            return LogicResult<SingleValueResult<string>>.Forbidden("Refresh token is expired");

        var accessToken = await tokens.CreateToken(Token.AccessTokenType, token.Owner);
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.TokenId.ToString(), new CookieOptions {
            MaxAge = options.Value.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });
        
        return LogicResult<SingleValueResult<string>>.Ok(accessToken.TokenId.ToString());
    }

    public async Task<LogicResult> Logout() {
        var accessToken = accessor.HttpContext?.User.GetAccessTokenId();
        var refreshToken = accessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            await tokens.DeleteUserTokens(tokenContext.User);
        
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