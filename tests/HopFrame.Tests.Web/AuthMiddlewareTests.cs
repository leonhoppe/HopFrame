using System.Security.Claims;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Claims;
using HopFrame.Web;
using HopFrame.Web.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace HopFrame.Tests.Web;

public class AuthMiddlewareTests {
    private readonly RequestDelegate _delegate = _ => Task.CompletedTask;
    
    public AuthMiddleware SetupEnvironment(bool isLoggedIn = true, Token newToken = null) {
        var auth = new Mock<IAuthService>();
        auth
            .Setup(a => a.IsLoggedIn())
            .ReturnsAsync(isLoggedIn);
        auth
            .Setup(a => a.RefreshLogin())
            .ReturnsAsync(newToken);

        var perms = new Mock<IPermissionRepository>();
        perms
            .Setup(p => p.GetFullPermissions(It.Is<User>(u => newToken.Owner.Id == u.Id)))
            .ReturnsAsync(CreateDummyUser().Permissions.Select(p => p.PermissionName).ToList);

        return new AuthMiddleware(auth.Object, perms.Object);
    }
    
    private User CreateDummyUser() => new() {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.Now,
        Email = "test@example.com",
        Username = "ExampleUser",
        Password = "1234567890",
        Permissions = new List<Permission> {
            new () {
                PermissionName = "test.permission"
            }
        }
    };

    [Fact]
    public async Task InvokeAsync_With_ValidLogin_Should_Succeed() {
        // Arrange
        var auth = SetupEnvironment();
        var context = new DefaultHttpContext();
        
        // Act
        await auth.InvokeAsync(context, _delegate);
        
        // Assert
        Assert.Null(context.User.FindFirst(HopFrameClaimTypes.UserId));
        Assert.Null(context.User.FindFirst(HopFrameClaimTypes.AccessTokenId));
        Assert.Null(context.User.FindFirst(HopFrameClaimTypes.Permission));
    }
    
    [Fact]
    public async Task InvokeAsync_With_InvalidLoginValidToken_Should_Succeed() {
        // Arrange
        var token = new Token {
            Content = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Type = Token.AccessTokenType,
            Owner = CreateDummyUser()
        };
        var auth = SetupEnvironment(false, token);
        var context = new DefaultHttpContext();
        
        // Act
        await auth.InvokeAsync(context, _delegate);
        
        // Assert
        Assert.Equal(token.Owner.Id.ToString(), context.User.FindFirstValue(HopFrameClaimTypes.UserId));
        Assert.Equal(token.Content.ToString(), context.User.FindFirstValue(HopFrameClaimTypes.AccessTokenId));
        Assert.Equal(token.Owner.Permissions.First().PermissionName, context.User.FindFirstValue(HopFrameClaimTypes.Permission));
    }
    
    [Fact]
    public async Task InvokeAsync_With_InvalidLoginInvalidToken_Should_Succeed() {
        // Arrange
        var auth = SetupEnvironment(false);
        var context = new DefaultHttpContext();
        
        // Act
        await auth.InvokeAsync(context, _delegate);
        
        // Assert
        Assert.Null(context.User.FindFirst(HopFrameClaimTypes.UserId));
        Assert.Null(context.User.FindFirst(HopFrameClaimTypes.AccessTokenId));
        Assert.Null(context.User.FindFirst(HopFrameClaimTypes.Permission));
    }
}