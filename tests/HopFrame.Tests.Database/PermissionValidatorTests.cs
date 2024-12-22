using HopFrame.Database;

namespace HopFrame.Tests.Database;

public class PermissionValidatorTests {

    [Fact]
    public void IncludesPermission_Returns_True_For_ExactPermission() {
        // Arrange
        var permissions = new [] { "test.permission", "test.permission.exact" };
        var permission = "test.permission.exact";

        // Act
        var hasPermission = PermissionValidator.IncludesPermission(permission, permissions);

        // Assert
        Assert.True(hasPermission);
    }
    
    [Fact]
    public void IncludesPermission_Returns_True_For_GroupPermission() {
        // Arrange
        var permissions = new [] { "test.permission.*", "test.permission.exact" };
        var permission = "test.permission.exact.other";

        // Act
        var hasPermission = PermissionValidator.IncludesPermission(permission, permissions);

        // Assert
        Assert.True(hasPermission);
    }
    
    [Fact]
    public void IncludesPermission_Returns_True_For_StarPermission() {
        // Arrange
        var permissions = new [] { "test.permission", "test.permission.exact", "*" };
        var permission = "test.permission.exact.other";

        // Act
        var hasPermission = PermissionValidator.IncludesPermission(permission, permissions);

        // Assert
        Assert.True(hasPermission);
    }
    
    [Fact]
    public void IncludesPermission_Returns_False() {
        // Arrange
        var permissions = new [] { "test.permission", "test.permission.exact" };
        var permission = "test.permission.exact.other";

        // Act
        var hasPermission = PermissionValidator.IncludesPermission(permission, permissions);

        // Assert
        Assert.False(hasPermission);
    }
    
}