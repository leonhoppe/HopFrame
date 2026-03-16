namespace HopFrame.Tests.Web.Services.Implementation;

using System.Security.Claims;
using Core.Configuration;
using HopFrame.Web.Services.Implementation;
using Microsoft.AspNetCore.Http;
using Moq;

public class AuthProviderTests {
    [Fact]
    public async Task IsAuthenticated_ReturnsTrue_WhenAnonymousAccessAllowed() {
        var config = new HopFrameConfig { AllowAnonymousAccess = true };
        var provider = new AuthProvider(CreateAccessor(null), config);

        var result = await provider.IsAuthenticated(null, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task IsAuthenticated_ReturnsFalse_WhenNoHttpContext() {
        var config = new HopFrameConfig { AllowAnonymousAccess = false };
        var provider = new AuthProvider(CreateAccessor(null), config);

        var result = await provider.IsAuthenticated(null, CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task IsAuthenticated_ReturnsTrue_WhenNoClaimRequired() {
        var config = new HopFrameConfig { AllowAnonymousAccess = false };
        var context = CreateContextWithClaims(new Claim("role", "admin"));

        var provider = new AuthProvider(CreateAccessor(context), config);

        var result = await provider.IsAuthenticated(null, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task IsAuthenticated_ReturnsTrue_WhenClaimExists() {
        var config = new HopFrameConfig {
            AllowAnonymousAccess = false,
            ClaimType = ClaimTypes.Role
        };

        var context = CreateContextWithClaims(new Claim(ClaimTypes.Role, "Editor"));

        var provider = new AuthProvider(CreateAccessor(context), config);

        var result = await provider.IsAuthenticated("Editor", CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task IsAuthenticated_ReturnsFalse_WhenClaimMissing() {
        var config = new HopFrameConfig {
            AllowAnonymousAccess = false,
            ClaimType = ClaimTypes.Role
        };

        var context = CreateContextWithClaims(new Claim(ClaimTypes.Role, "User"));

        var provider = new AuthProvider(CreateAccessor(context), config);

        var result = await provider.IsAuthenticated("Admin", CancellationToken.None);

        Assert.False(result);
    }

    // -------------------------
    // Helpers
    // -------------------------

    private static DefaultHttpContext CreateContextWithClaims(params Claim[] claims) {
        var context = new DefaultHttpContext {
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
        };
        return context;
    }

    private static IHttpContextAccessor CreateAccessor(HttpContext? context) {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(a => a.HttpContext).Returns(context);
        return accessor.Object;
    }
}