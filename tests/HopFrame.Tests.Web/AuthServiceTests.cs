using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
using HopFrame.Tests.Web.Extensions;
using HopFrame.Web.Services;
using HopFrame.Web.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace HopFrame.Tests.Web;

public class AuthServiceTests {
    private readonly Guid _refreshToken = Guid.NewGuid();
    private readonly Guid _accessToken = Guid.NewGuid();

    private (IAuthService, HttpContext) SetupEnvironment(bool passwordIsCorrect = true, Token providedRefreshToken = null, string providedTokenCookie = null, Token providedAccessToken = null) {
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
                TokenId = _refreshToken,
                Type = Token.RefreshTokenType
            });
        tokens
            .Setup(t => t.CreateToken(It.Is<int>(t => t == Token.AccessTokenType), It.IsAny<User>()))
            .ReturnsAsync(new Token {
                TokenId = _accessToken,
                Type = Token.AccessTokenType
            });
        tokens
            .Setup(t => t.GetToken(It.Is<string>(token => token == _refreshToken.ToString())))
            .ReturnsAsync(providedRefreshToken);

        var context = new Mock<ITokenContext>();
        context
            .Setup(c => c.User)
            .Returns(CreateDummyUser());
        context
            .Setup(c => c.AccessToken)
            .Returns(providedAccessToken);

        return (new AuthService(users.Object, accessor, tokens.Object, context.Object, new OptionsWrapper<HopFrameAuthenticationOptions>(new HopFrameAuthenticationOptions())), accessor.HttpContext);
    }

    private User CreateDummyUser() => new() {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.Now,
        Email = "test@example.com",
        Username = "ExampleUser",
        Password = "1234567890"
    };

    [Fact]
    public async Task Register_Should_Succeed() {
        // Arrange
        var (service, context) = SetupEnvironment();
        var register = new UserRegister {
            Email = CreateDummyUser().Email,
            Username = CreateDummyUser().Username,
            Password = CreateDummyUser().Password
        };
        
        // Act
        await service.Register(register);
        
        // Assert
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task Login_Should_Succeed() {
        // Arrange
        var (service, context) = SetupEnvironment();
        var login = new UserLogin {
            Email = CreateDummyUser().Email,
            Password = CreateDummyUser().Password
        };
        
        // Act
        var result = await service.Login(login);
        
        // Assert
        Assert.True(result);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Equal(_refreshToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }
    
    [Fact]
    public async Task Login_With_WrongPassword_Should_Fail() {
        // Arrange
        var (service, context) = SetupEnvironment(false);
        var login = new UserLogin {
            Email = CreateDummyUser().Email,
            Password = CreateDummyUser().Password
        };
        
        // Act
        var result = await service.Login(login);
        
        // Assert
        Assert.False(result);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }
    
    [Fact]
    public async Task Login_With_WrongEmail_Should_Fail() {
        // Arrange
        var (service, context) = SetupEnvironment();
        var login = new UserLogin {
            Email = "wrong@example.com",
            Password = CreateDummyUser().Password
        };

        // Act
        var result = await service.Login(login);
        
        // Assert
        Assert.False(result);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task Logout_Should_Succeed() {
        // Arrange
        var (service, context) = SetupEnvironment(providedTokenCookie: _refreshToken.ToString());
        context.Response.Cookies.Append(ITokenContext.AccessTokenType, _accessToken.ToString());
        context.Response.Cookies.Append(ITokenContext.RefreshTokenType, _refreshToken.ToString());
        
        // Act
        await service.Logout();
        
        // Assert
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.RefreshTokenType));
    }

    [Fact]
    public async Task RefreshLogin_Should_Succeed() {
        // Arrange
        var token = new Token {
            Type = Token.RefreshTokenType,
            TokenId = _refreshToken,
            CreatedAt = DateTime.Now,
            Owner = CreateDummyUser()
        };
        var (service, context) = SetupEnvironment(true, token, token.TokenId.ToString());
        
        // Act
        var result = await service.RefreshLogin();
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(_accessToken, result.TokenId);
        Assert.Equal(_accessToken.ToString(), context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }
    
    [Fact]
    public async Task RefreshLogin_With_NoProvidedToken_Should_Fail() {
        // Arrange
        var (service, context) = SetupEnvironment();
        
        // Act
        var result = await service.RefreshLogin();
        
        // Assert
        Assert.Null(result);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }
    
    [Fact]
    public async Task RefreshLogin_With_WrongToken_Should_Fail() {
        // Arrange
        var (service, context) = SetupEnvironment(true, null, _refreshToken.ToString());
        
        // Act
        var result = await service.RefreshLogin();
        
        // Assert
        Assert.Null(result);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }
    
    [Fact]
    public async Task RefreshLogin_With_WrongTokenType_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.AccessTokenType,
            TokenId = _refreshToken,
            CreatedAt = DateTime.Now,
            Owner = CreateDummyUser()
        };
        var (service, context) = SetupEnvironment(true, token, token.TokenId.ToString());
        
        // Act
        var result = await service.RefreshLogin();
        
        // Assert
        Assert.Null(result);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }
    
    [Fact]
    public async Task RefreshLogin_With_ExpiredToken_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.RefreshTokenType,
            TokenId = _refreshToken,
            CreatedAt = DateTime.MinValue,
            Owner = CreateDummyUser()
        };
        var (service, context) = SetupEnvironment(true, token, token.TokenId.ToString());
        
        // Act
        var result = await service.RefreshLogin();
        
        // Assert
        Assert.Null(result);
        Assert.Null(context.Response.Headers.FindCookie(ITokenContext.AccessTokenType));
    }

    [Fact]
    public async Task IsLoggedIn_Should_Succeed() {
        // Arrange
        var token = new Token {
            Type = Token.AccessTokenType,
            TokenId = _accessToken,
            CreatedAt = DateTime.Now,
            Owner = CreateDummyUser()
        };
        var (service, context) = SetupEnvironment(providedAccessToken: token);
        
        // Act
        var result = await service.IsLoggedIn();
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsLoggedIn_With_NoProvidedToken_Should_Fail() {
        // Arrange
        var (service, context) = SetupEnvironment();
        
        // Act
        var result = await service.IsLoggedIn();
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task IsLoggedIn_With_WrongTokenType_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.RefreshTokenType,
            TokenId = _accessToken,
            CreatedAt = DateTime.Now,
            Owner = CreateDummyUser()
        };
        var (service, context) = SetupEnvironment(providedAccessToken: token);
        
        // Act
        var result = await service.IsLoggedIn();
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task IsLoggedIn_With_ExpiredToken_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.AccessTokenType,
            TokenId = _accessToken,
            CreatedAt = DateTime.MinValue,
            Owner = CreateDummyUser()
        };
        var (service, context) = SetupEnvironment(providedAccessToken: token);
        
        // Act
        var result = await service.IsLoggedIn();
        
        // Assert
        Assert.False(result);
    }
    
    [Fact]
    public async Task IsLoggedIn_With_NoOwner_Should_Fail() {
        // Arrange
        var token = new Token {
            Type = Token.AccessTokenType,
            TokenId = _accessToken,
            CreatedAt = DateTime.Now,
            Owner = null
        };
        var (service, context) = SetupEnvironment(providedAccessToken: token);
        
        // Act
        var result = await service.IsLoggedIn();
        
        // Assert
        Assert.False(result);
    }
}