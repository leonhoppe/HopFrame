using HopFrame.Api.Models;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using Microsoft.AspNetCore.Http;

namespace HopFrame.Api.Logic.Implementation;

public class AuthLogic(IUserRepository users, ITokenRepository tokens, ITokenContext tokenContext, IHttpContextAccessor accessor) : IAuthLogic {
    
    public async Task<LogicResult<SingleValueResult<string>>> Login(UserLogin login) {
        var user = await users.GetUserByEmail(login.Email);

        if (user is null)
            return LogicResult<SingleValueResult<string>>.NotFound("The provided email address was not found");

        if (!await users.CheckUserPassword(user, login.Password))
            return LogicResult<SingleValueResult<string>>.Forbidden("The provided password is not correct");

        var refreshToken = await tokens.CreateToken(Token.RefreshTokenType, user);
        var accessToken = await tokens.CreateToken(Token.AccessTokenType, user);
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.AccessTokenTime,
            HttpOnly = true,
            Secure = true
        });

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Content.ToString());
    }

    public async Task<LogicResult<SingleValueResult<string>>> Register(UserRegister register) {
        if (register.Password.Length < 8)
            return LogicResult<SingleValueResult<string>>.Conflict("Password needs to be at least 8 characters long");

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
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.RefreshTokenType, refreshToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.RefreshTokenTime,
            HttpOnly = true,
            Secure = true
        });
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });

        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Content.ToString());
    }

    public async Task<LogicResult<SingleValueResult<string>>> Authenticate() {
        var refreshToken = accessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
        if (string.IsNullOrEmpty(refreshToken))
            return LogicResult<SingleValueResult<string>>.Conflict("Refresh token not provided");

        var token = await tokens.GetToken(refreshToken);

        if (token.Type != Token.RefreshTokenType)
            return LogicResult<SingleValueResult<string>>.BadRequest("The provided token is not a refresh token");

        if (token is null)
            return LogicResult<SingleValueResult<string>>.NotFound("Refresh token not valid");

        if (token.CreatedAt + HopFrameAuthentication.RefreshTokenTime < DateTime.Now)
            return LogicResult<SingleValueResult<string>>.Conflict("Refresh token is expired");

        var accessToken = await tokens.CreateToken(Token.AccessTokenType, token.Owner);
        
        accessor.HttpContext?.Response.Cookies.Append(ITokenContext.AccessTokenType, accessToken.Content.ToString(), new CookieOptions {
            MaxAge = HopFrameAuthentication.AccessTokenTime,
            HttpOnly = false,
            Secure = true
        });
        
        return LogicResult<SingleValueResult<string>>.Ok(accessToken.Content.ToString());
    }

    public async Task<LogicResult> Logout() {
        var accessToken = accessor.HttpContext?.User.GetAccessTokenId();
        var refreshToken = accessor.HttpContext?.Request.Cookies[ITokenContext.RefreshTokenType];
        
        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            return LogicResult.Conflict("access or refresh token not provided");

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