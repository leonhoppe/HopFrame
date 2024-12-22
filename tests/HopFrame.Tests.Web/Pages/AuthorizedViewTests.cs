using System.Security.Claims;
using Bunit;
using Bunit.TestDoubles;
using HopFrame.Security.Authentication;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Claims;
using HopFrame.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace HopFrame.Tests.Web.Pages;

public class AuthorizedViewTests : TestContext {
    private readonly string _testRedirect = "testRedirect";
    private readonly string _testPermission = "test.permission";
    private readonly string _innerHtml = "<p>Inner Render</p>";

    public NavigationManager SetupEnvironment(bool authenticated = true, params string[] userPermissions) {
        var auth = new Mock<ITokenContext>();
        auth
            .Setup(a => a.IsAuthenticated)
            .Returns(authenticated);

        var context = new DefaultHttpContext();
        var claims = userPermissions?.Select(perm => new Claim(HopFrameClaimTypes.Permission, perm)).ToList();
        context.User.AddIdentity(new ClaimsIdentity(claims, HopFrameAuthentication.SchemeName));
        var accessor = new Mock<IHttpContextAccessor>();
        accessor
            .Setup(a => a.HttpContext)
            .Returns(context);

        Services.AddSingleton(auth.Object);
        Services.AddSingleton(accessor.Object);
        return Services.GetRequiredService<FakeNavigationManager>();
    }

    [Fact]
    public void AuthorizedView_With_NoValidLogin_And_Redirection_Should_Redirect() {
        // Arrange
        var navigator = SetupEnvironment(false);
        
        // Act
        RenderComponent<AuthorizedView>(parameters => parameters
            .Add(a => a.RedirectIfUnauthorized, _testRedirect));
        
        // Assert
        Assert.EndsWith(_testRedirect, navigator.Uri);
    }
    
    [Fact]
    public void AuthorizedView_With_NoPermissions_And_Redirection_Should_Redirect() {
        // Arrange
        var navigator = SetupEnvironment();
        
        // Act
        RenderComponent<AuthorizedView>(parameters => parameters
            .Add(a => a.RedirectIfUnauthorized, _testRedirect)
            .Add(a => a.Permission, _testPermission));
        
        // Assert
        Assert.EndsWith(_testRedirect, navigator.Uri);
    }
    
    [Fact]
    public void AuthorizedView_With_FewPermissions_And_Redirection_Should_Redirect() {
        // Arrange
        var navigator = SetupEnvironment(true, "other.permission");
        
        // Act
        RenderComponent<AuthorizedView>(parameters => parameters
            .Add(a => a.RedirectIfUnauthorized, _testRedirect)
            .Add(a => a.Permissions, [_testPermission, "other.permission"]));
        
        // Assert
        Assert.EndsWith(_testRedirect, navigator.Uri);
    }
    
    [Fact]
    public void AuthorizedView_With_Permissions_And_Redirection_Should_NotRedirect() {
        // Arrange
        var navigator = SetupEnvironment(true, _testPermission);
        
        // Act
        RenderComponent<AuthorizedView>(parameters => parameters
            .Add(a => a.RedirectIfUnauthorized, _testRedirect)
            .Add(a => a.Permission, _testPermission));
        
        // Assert
        Assert.False(navigator.Uri.EndsWith(_testRedirect));
    }
    
    [Fact]
    public void AuthorizedView_With_AllPermissions_And_Redirection_Should_NotRedirect() {
        // Arrange
        var navigator = SetupEnvironment(true, _testPermission, "other.permission");
        
        // Act
        RenderComponent<AuthorizedView>(parameters => parameters
            .Add(a => a.RedirectIfUnauthorized, _testRedirect)
            .Add(a => a.Permissions, [_testPermission, "other.permission"]));
        
        // Assert
        Assert.False(navigator.Uri.EndsWith(_testRedirect));
    }

    [Fact]
    public void AuthorizedView_With_ChildComponent_And_ValidLogin_Should_DisplayChildren() {
        // Arrange
        SetupEnvironment();
        
        // Act
        var component = RenderComponent<AuthorizedView>(parameters => parameters
            .AddChildContent(_innerHtml));
        
        // Assert
        Assert.Contains(_innerHtml, component.Markup);
    }
    
    [Fact]
    public void AuthorizedView_With_ChildComponent_And_InvalidLogin_Should_NotDisplayChildren() {
        // Arrange
        SetupEnvironment(false);
        
        // Act
        var component = RenderComponent<AuthorizedView>(parameters => parameters
            .AddChildContent(_innerHtml));
        
        // Assert
        Assert.DoesNotContain(_innerHtml, component.Markup);
    }
    
}