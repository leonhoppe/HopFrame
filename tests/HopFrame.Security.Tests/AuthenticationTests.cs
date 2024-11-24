using System.Text.Encodings.Web;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace HopFrame.Security.Tests;

public class AuthenticationTests {

    private async Task<HopFrameAuthentication> SetupEnvironment(Token correctToken = null, string providedToken = null) {
        var options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();
        options
            .Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new AuthenticationSchemeOptions());

        var logger = new Mock<ILoggerFactory>();
        logger
            .Setup(x => x.CreateLogger(It.IsAny<String>()))
            .Returns(new Mock<ILogger<HopFrameAuthentication>>().Object);

        var encoder = new Mock<UrlEncoder>();
        var clock = new Mock<ISystemClock>();
        var tokens = new Mock<ITokenRepository>();
        var perms = new Mock<IPermissionRepository>();

        var provideCorrectToken = correctToken is null;
        correctToken ??= new Token {
            Content = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Type = Token.AccessTokenType,
            Owner = new User {
                Id = Guid.NewGuid()
            }
        };

        tokens
            .Setup(x => x.GetToken(It.Is<string>(t => t == correctToken.Content.ToString())))
            .ReturnsAsync(correctToken);

        perms
            .Setup(x => x.GetFullPermissions(It.IsAny<User>()))
            .ReturnsAsync(new List<string>());

        var auth = new HopFrameAuthentication(options.Object, logger.Object, encoder.Object, clock.Object, tokens.Object, perms.Object);
        var context = new DefaultHttpContext();
        if (provideCorrectToken)
            context.HttpContext.Request.Headers.Append(HopFrameAuthentication.SchemeName, correctToken.Content.ToString());
        if (providedToken is not null)
            context.HttpContext.Request.Headers.Append(HopFrameAuthentication.SchemeName, providedToken);
        
        await auth.InitializeAsync(new AuthenticationScheme(HopFrameAuthentication.SchemeName, null, typeof(HopFrameAuthentication)), context);
        return auth;
    }

    [Fact]
    public async Task Authentication_Should_Succeed() {
        // Arrange
        var auth = await SetupEnvironment();
        
        // Act
        var result = await auth.AuthenticateAsync();
        
        // Assert
        Assert.True(result.Succeeded);
    }
    
    [Fact]
    public async Task Authentication_With_NoToken_Should_Fail() {
        // Arrange
        var auth = await SetupEnvironment(new Token());
        
        // Act
        var result = await auth.AuthenticateAsync();
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal("No Access Token provided", result.Failure.Message);
    }
    
    [Fact]
    public async Task Authentication_With_InvalidToken_Should_Fail() {
        // Arrange
        var auth = await SetupEnvironment(null, Guid.NewGuid().ToString());
        
        // Act
        var result = await auth.AuthenticateAsync();
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal("The provided Access Token does not exist", result.Failure.Message);
    }
    
    [Fact]
    public async Task Authentication_With_ExpiredToken_Should_Fail() {
        // Arrange
        var token = new Token {
            Content = Guid.NewGuid(),
            CreatedAt = DateTime.MinValue,
            Type = Token.AccessTokenType,
            Owner = new User()
        };
        var auth = await SetupEnvironment(token, token.Content.ToString());
        
        // Act
        var result = await auth.AuthenticateAsync();
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal("The provided Access Token is expired", result.Failure.Message);
    }
    
    [Fact]
    public async Task Authentication_With_UnownedToken_Should_Fail() {
        // Arrange
        var token = new Token {
            Content = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Type = Token.AccessTokenType,
            Owner = null
        };
        var auth = await SetupEnvironment(token, token.Content.ToString());
        
        // Act
        var result = await auth.AuthenticateAsync();
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.NotNull(result.Failure);
        Assert.Equal("The provided Access Token does not match any user", result.Failure.Message);
    }
    
    
}