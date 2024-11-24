using System.Security.Claims;
using HopFrame.Security.Authentication;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;

namespace HopFrame.Security.Tests;

public class AuthorizationTests {

    private (AuthorizedFilter, AuthorizationFilterContext) SetupEnvironment(string[] userPermissions, string[] requiredPermissions, bool accessTokenProvided = true) {
        var filter = new AuthorizedFilter(requiredPermissions);
        
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext { HttpContext = httpContext, RouteData = new RouteData(), ActionDescriptor = new ActionDescriptor() };
        var context = new Mock<AuthorizationFilterContext>(MockBehavior.Default, actionContext, new List<IFilterMetadata>());

        context
            .Setup(x => x.Filters)
            .Returns(new List<IFilterMetadata>());

        context.SetupProperty(c => c.Result);

        var claims = new List<Claim> {
            new(HopFrameClaimTypes.UserId, Guid.NewGuid().ToString())
        };
        if (accessTokenProvided)
            claims.Add(new (HopFrameClaimTypes.AccessTokenId, Guid.NewGuid().ToString()));
        claims.AddRange(userPermissions.Select(perm => new Claim(HopFrameClaimTypes.Permission, perm)));

        context.Object.HttpContext.User.AddIdentity(new ClaimsIdentity(claims, HopFrameAuthentication.SchemeName));

        return (filter, context.Object);
    }

    [Fact]
    public void OnAuthorization_Should_Succeed() {
        // Arrange
        var (filter, context) = SetupEnvironment(["test.permission"], ["test.permission"]);
        
        // Act
        filter.OnAuthorization(context);
        
        // Assert
        Assert.Null(context.Result);
    }

    [Fact]
    public void OnAuthorization_With_NoToken_Should_Fail() {
        // Arrange
        var (filter, context) = SetupEnvironment([], [], false);
        
        // Act
        filter.OnAuthorization(context);
        
        // Assert
        Assert.NotNull(context.Result);
        Assert.IsType<UnauthorizedResult>(context.Result);
    }
    
    [Fact]
    public void OnAuthorization_With_NoPermissions_Should_Fail() {
        // Arrange
        var (filter, context) = SetupEnvironment([], ["test.permission"]);
        
        // Act
        filter.OnAuthorization(context);
        
        // Assert
        Assert.NotNull(context.Result);
        Assert.IsType<UnauthorizedResult>(context.Result);
    }
    
    [Fact]
    public void OnAuthorization_With_InsufficientPermissions_Should_Fail() {
        // Arrange
        var (filter, context) = SetupEnvironment(["permission.other"], ["test.permission"]);
        
        // Act
        filter.OnAuthorization(context);
        
        // Assert
        Assert.NotNull(context.Result);
        Assert.IsType<UnauthorizedResult>(context.Result);
    }
    
}