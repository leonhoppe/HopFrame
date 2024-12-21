using System.Net;
using System.Security.Claims;
using HopFrame.Api.Logic;
using HopFrame.Api.Logic.Implementation;
using HopFrame.Api.Models;
using HopFrame.Tests.Api.Extensions;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using Microsoft.AspNetCore.Http;
using Moq;

namespace HopFrame.Tests.Api;

public class AuthLogicTests {

    private readonly Guid _refreshToken = Guid.NewGuid();
    private readonly Guid _accessToken = Guid.NewGuid();

    private (IAuthLogic, HttpContext) SetupEnvironment(bool passwordIsCorrect = true, Token providedRefreshToken = null, string providedTokenCookie = null, bool provideAccessToken = true) {
        var accessor = new HttpContextAccessor {
            HttpContext = new DefaultHttpContext()
        };

        if (providedTokenCookie != null) {
            var cookies = new Mock<IRequestCookieCollection>();
            cookies
                .SetupGet(c => c[ITokenContext.RefreshTokenType])
                .Returns(providedTokenCookie);
            accessor.HttpContext.Request.Cookies = cookies.Object;
        }

        if (provideAccessToken) {
            var claims = new List<Claim> {
                new(HopFrameClaimTypes.AccessTokenId, _accessToken.ToString())
            };
            accessor.HttpContext.User.AddIdentity(new ClaimsIdentity(claims, HopFrameAuthentication.SchemeName));
        }

        var users = new Mock<IUserRepository>();
        users
            .Setup(u => u.GetUserByEmail(It.Is<string>(email => CreateDummyUser().Email == email)))
            .ReturnsAsync(CreateDummyUser());
        users
            .Setup(u => u.CheckUserPassword(It.Is<User>(u => u.Email == CreateDummyUser().Email), It.IsAny<string>()))
            .ReturnsAsync(passwordIsCorrect);
        users
            .Setup(u => u.AddUser(It.IsAny<User>()))
            .ReturnsAsync(CreateDummyUser());
        users
            .Setup(u => u.GetUsers())
            .ReturnsAsync(new List<User> { CreateDummyUser() });

        var tokens = new Mock<ITokenRepository>();
        tokens
            .Setup(t => t.CreateToken(It.Is<int>(t => t == Token.RefreshTokenType), It.IsAny<User>()))
            .ReturnsAsync(new Token {
                Content = _refreshToken,
                Type = Token.RefreshTokenType
            });
        tokens
            .Setup(t => t.CreateToken(It.Is<int>(t => t == Token.AccessTokenType), It.IsAny<User>()))
            .ReturnsAsync(new Token {
                Content = _accessToken,
                Type = Token.AccessTokenType
            });
        tokens
            .Setup(t => t.GetToken(It.Is<string>(token => token == _refreshToken.ToString())))
            .ReturnsAsync(providedRefreshToken);

        var context = new Mock<ITokenContext>();
        context
            .Setup(c => c.User)
            .Returns(CreateDummyUser());

        return (new AuthLogic(users.Object, tokens.Object, context.Object, accessor), accessor.HttpContext);
    }

    private User CreateDummyUser() => new() {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.Now,
        Email = "test@example.com",
        Username = "ExampleUser",
        Password = "1234567890"
    };

    [Fact]
    public async Task Login_Should_Succeed() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        var login = new UserLogin {
            Email = CreateDummyUser().Email,
            Password = CreateDummyUser().Password
        };

        // Act
        var result = await auth.Login(login);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(_accessToken.ToString(), result.Data.Value);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Login_With_WrongEmail_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        var login = new UserLogin {
            Email = "wrong@example.com",
            Password = CreateDummyUser().Password
        };

        // Act
        var result = await auth.Login(login);
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.NotFound, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Login_With_WrongPassword_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment(false);
        var login = new UserLogin {
            Email = CreateDummyUser().Email,
            Password = CreateDummyUser().Password
        };

        // Act
        var result = await auth.Login(login);
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Forbidden, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Register_Should_Succeed() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        var register = new UserRegister {
            Email = "new@example.com",
            Password = "1234567890",
            Username = "NewUser"
        };
        
        // Act
        var result = await auth.Register(register);
        
        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(_accessToken.ToString(), result.Data.Value);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Register_With_ShortPassword_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        var register = new UserRegister {
            Email = "new@example.com",
            Password = "12345",
            Username = "NewUser"
        };
        
        // Act
        var result = await auth.Register(register);
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.BadRequest, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Register_With_ExistingUsername_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        var register = new UserRegister {
            Email = "new@example.com",
            Password = "1234567890",
            Username = CreateDummyUser().Username
        };
        
        // Act
        var result = await auth.Register(register);
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Conflict, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }
    
    [Fact]
    public async Task Register_With_ExistingEmail_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        var register = new UserRegister {
            Email = CreateDummyUser().Email,
            Password = "1234567890",
            Username = "NewUser"
        };
        
        // Act
        var result = await auth.Register(register);
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Conflict, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Authenticate_Should_Succeed() {
        // Arrange
        var token = new Token {
            Type = Token.RefreshTokenType,
            Content = _refreshToken,
            CreatedAt = DateTime.Now,
            Owner = CreateDummyUser()
        };
        var (auth, context) = SetupEnvironment(true, token, token.Content.ToString());
        
        // Act
        var result = await auth.Authenticate();
        
        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(_accessToken.ToString(), result.Data.Value);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Authenticate_With_NoProvidedToken_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        
        // Act
        var result = await auth.Authenticate();
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.BadRequest, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }
    
    [Fact]
    public async Task Authenticate_With_WrongToken_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment(true, null, _refreshToken.ToString());
        
        // Act
        var result = await auth.Authenticate();
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.NotFound, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Authenticate_With_WrongTokenType_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.AccessTokenType,
            Content = _refreshToken,
            CreatedAt = DateTime.Now,
            Owner = CreateDummyUser()
        };
        var (auth, context) = SetupEnvironment(true, token, token.Content.ToString());
        
        // Act
        var result = await auth.Authenticate();
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Conflict, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Authenticate_With_ExpiredToken_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.RefreshTokenType,
            Content = _refreshToken,
            CreatedAt = DateTime.MinValue,
            Owner = CreateDummyUser()
        };
        var (auth, context) = SetupEnvironment(true, token, token.Content.ToString());
        
        // Act
        var result = await auth.Authenticate();
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Forbidden, result.State);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Logout_Should_Succeed() {
        // Arrange
        var (auth, context) = SetupEnvironment(providedTokenCookie: _refreshToken.ToString());
        context.Response.Cookies.Append(ITokenContext.AccessTokenType, _accessToken.ToString());
        context.Response.Cookies.Append(ITokenContext.RefreshTokenType, _refreshToken.ToString());
        
        // Act
        var result = await auth.Logout();
        
        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Logout_With_NoAccessToken_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment(provideAccessToken: false);
        context.Response.Cookies.Append(ITokenContext.AccessTokenType, _accessToken.ToString());
        context.Response.Cookies.Append(ITokenContext.RefreshTokenType, _refreshToken.ToString());
        
        // Act
        var result = await auth.Logout();
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Conflict, result.State);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Logout_With_NoRefreshToken_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        context.Response.Cookies.Append(ITokenContext.AccessTokenType, _accessToken.ToString());
        context.Response.Cookies.Append(ITokenContext.RefreshTokenType, _refreshToken.ToString());
        
        // Act
        var result = await auth.Logout();
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Conflict, result.State);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Delete_Should_Succeed() {
        // Arrange
        var (auth, context) = SetupEnvironment();
        context.Response.Cookies.Append(ITokenContext.AccessTokenType, _accessToken.ToString());
        context.Response.Cookies.Append(ITokenContext.RefreshTokenType, _refreshToken.ToString());
        var validation = new UserPasswordValidation {
            Password = CreateDummyUser().Password
        };

        // Act
        var result = await auth.Delete(validation);
        
        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Delete_With_WrongPassword_Should_Fail() {
        // Arrange
        var (auth, context) = SetupEnvironment(false);
        context.Response.Cookies.Append(ITokenContext.AccessTokenType, _accessToken.ToString());
        context.Response.Cookies.Append(ITokenContext.RefreshTokenType, _refreshToken.ToString());
        var validation = new UserPasswordValidation {
            Password = CreateDummyUser().Password
        };

        // Act
        var result = await auth.Delete(validation);
        
        // Assert
        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Forbidden, result.State);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

}