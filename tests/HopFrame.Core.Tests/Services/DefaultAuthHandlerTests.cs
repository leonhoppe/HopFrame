using HopFrame.Core.Services.Implementations;

namespace HopFrame.Core.Tests.Services;

public class DefaultAuthHandlerTests {
    [Fact]
    public async Task IsAuthenticatedAsync_ReturnsTrue() {
        // Arrange
        var authHandler = new DefaultAuthHandler();

        // Act
        var result = await authHandler.IsAuthenticatedAsync(null);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
    public async Task IsAuthenticatedAsync_WithPolicy_ReturnsTrue() {
        // Arrange
        var authHandler = new DefaultAuthHandler();

        // Act
        var result = await authHandler.IsAuthenticatedAsync("TestPolicy");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task GetCurrentUserDisplayNameAsync_ReturnsEmptyString() {
        // Arrange
        var authHandler = new DefaultAuthHandler();

        // Act
        var result = await authHandler.GetCurrentUserDisplayNameAsync();

        // Assert
        Assert.Equal(string.Empty, result);
    }
}