using HopFrame.Api.Controller;
using HopFrame.Api.Models;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authentication.OpenID.Models;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HopFrame.Tests.Api.Controllers;

public class OpenIdControllerTests {
    private (Mock<IOpenIdAccessor>, OpenIdController) SetupEnvironment(out HttpContext httpContext) {
        var mockAccessor = new Mock<IOpenIdAccessor>();
        var controller = new OpenIdController(mockAccessor.Object);

        httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext {
            HttpContext = httpContext
        };
        return (mockAccessor, controller);
    }

    [Fact]
    public async Task RedirectToProvider_ShouldRedirect_WhenPerformRedirectIsTrue() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out _);
        var uri = "https://example.com/auth";
        mockAccessor.Setup(a => a.ConstructAuthUri(It.IsAny<string>())).ReturnsAsync(uri);

        // Act
        var result = await controller.RedirectToProvider("https://redirectafter.com", 1);

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal(uri, redirectResult.Url);
    }

    [Fact]
    public async Task RedirectToProvider_ShouldReturnOk_WhenPerformRedirectIsFalse() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out _);
        var uri = "https://example.com/auth";
        mockAccessor.Setup(a => a.ConstructAuthUri(It.IsAny<string>())).ReturnsAsync(uri);

        // Act
        var result = await controller.RedirectToProvider("https://redirectafter.com", 0);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var singleValueResult = Assert.IsType<SingleValueResult<string>>(okResult.Value);
        Assert.Equal(uri, singleValueResult.Value);
    }
    
        [Fact]
    public async Task Callback_ShouldReturnBadRequest_WhenAuthorizationCodeIsMissing() {
        // Arrange
        var (_, controller) = SetupEnvironment(out _);

        // Act
        var result = await controller.Callback(string.Empty, "state");

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Authorization code is missing", badRequestResult.Value);
    }

    [Fact]
    public async Task Callback_ShouldReturnForbidden_WhenAuthorizationCodeIsNotValid() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out _);
        mockAccessor.Setup(a => a.RequestToken(It.IsAny<string>())).ReturnsAsync((OpenIdToken)null);

        // Act
        var result = await controller.Callback("invalid_code", "state");

        // Assert
        var forbidResult = Assert.IsType<ForbidResult>(result);
        Assert.Equal("Authorization code is not valid", forbidResult.AuthenticationSchemes.First());
    }

    [Fact]
    public async Task Callback_ShouldReturnOk_WhenStateIsNull() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out _);
        var token = new OpenIdToken { AccessToken = "valid_token" };
        mockAccessor.Setup(a => a.RequestToken(It.IsAny<string>())).ReturnsAsync(token);

        // Act
        var result = await controller.Callback("valid_code", null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var singleValueResult = Assert.IsType<SingleValueResult<string>>(okResult.Value);
        Assert.Equal("valid_token", singleValueResult.Value);
    }

    [Fact]
    public async Task Callback_ShouldRedirect_WhenStateIsProvided() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out _);
        var token = new OpenIdToken { AccessToken = "valid_token" };
        mockAccessor.Setup(a => a.RequestToken(It.IsAny<string>())).ReturnsAsync(token);

        // Act
        var result = await controller.Callback("valid_code", "https://redirect.com/{token}");

        // Assert
        var redirectResult = Assert.IsType<RedirectResult>(result);
        Assert.Equal("https://redirect.com/valid_token", redirectResult.Url);
    }
    
    [Fact]
    public async Task Refresh_ShouldReturnBadRequest_WhenRefreshTokenNotProvided() {
        // Arrange
        var (_, controller) = SetupEnvironment(out _);

        // Act
        var result = await controller.Refresh();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Refresh token not provided", badRequestResult.Value);
    }

    [Fact]
    public async Task Refresh_ShouldReturnConflict_WhenRefreshTokenNotValid() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out var httpContext);
        var cookies = new Mock<IRequestCookieCollection>();
        cookies
            .SetupGet(c => c[ITokenContext.RefreshTokenType])
            .Returns("invalid_token");
        httpContext.Request.Cookies = cookies.Object;
        mockAccessor.Setup(a => a.RefreshAccessToken(It.IsAny<string>())).ReturnsAsync((OpenIdToken)null);

        // Act
        var result = await controller.Refresh();

        // Assert
        var conflictResult = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("Refresh token not valid", conflictResult.Value);
    }

    [Fact]
    public async Task Refresh_ShouldReturnOk_WhenRefreshTokenIsValid() {
        // Arrange
        var (mockAccessor, controller) = SetupEnvironment(out var httpContext);
        var cookies = new Mock<IRequestCookieCollection>();
        cookies
            .SetupGet(c => c[ITokenContext.RefreshTokenType])
            .Returns("valid_token");
        httpContext.Request.Cookies = cookies.Object;
        var token = new OpenIdToken { AccessToken = "new_access_token", RefreshToken = "new_refresh_token", ExpiresIn = 3600 };
        mockAccessor.Setup(a => a.RefreshAccessToken(It.IsAny<string>())).ReturnsAsync(token);

        // Act
        var result = await controller.Refresh();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var singleValueResult = Assert.IsType<SingleValueResult<string>>(okResult.Value);
        Assert.Equal("new_access_token", singleValueResult.Value);
    }
    
    [Fact]
    public void Logout_ShouldReturnOk() {
        // Arrange
        var (_, controller) = SetupEnvironment(out _);

        // Act
        var result = controller.Logout();

        // Assert
        Assert.IsType<OkResult>(result);
    }
}
