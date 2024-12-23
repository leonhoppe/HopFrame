using System.Net;
using HopFrame.Api.Controller;
using HopFrame.Api.Logic;
using HopFrame.Api.Models;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace HopFrame.Tests.Api.Controllers;

public class UserControllerTests {
    private (Mock<IUserLogic>, Mock<IOptions<AdminPermissionOptions>>, Mock<IPermissionRepository>, Mock<ITokenContext>, UserController) SetupEnvironment() {
        var mockLogic = new Mock<IUserLogic>();
        var mockPermissions = new Mock<IOptions<AdminPermissionOptions>>();
        var mockPerms = new Mock<IPermissionRepository>();
        var mockContext = new Mock<ITokenContext>();

        var options = new AdminPermissionOptions();

        mockPermissions.Setup(o => o.Value).Returns(options);
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
        mockContext
            .Setup(c => c.User)
            .Returns(new User { Id = Guid.NewGuid(), Username = "user" });

        var controller =
            new UserController(mockPermissions.Object, mockPerms.Object, mockContext.Object, mockLogic.Object);

        return (mockLogic, mockPermissions, mockPerms, mockContext, controller);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetUsers();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsers_WhenUserIsAuthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var users = new List<User> { new User { Username = "testuser" } };
        mockLogic.Setup(l => l.GetUsers()).ReturnsAsync(LogicResult<IList<User>>.Ok(users));

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IList<User>>(okResult.Value);
        Assert.Single(returnedUsers);
        Assert.Equal("testuser", returnedUsers.First().Username);
    }

    [Fact]
    public async Task GetUsers_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var users = new List<User> { new User { Username = "testuser" } };
        mockLogic.Setup(l => l.GetUsers()).ReturnsAsync(LogicResult<IList<User>>.Ok(users));

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUsers = Assert.IsAssignableFrom<IList<User>>(okResult.Value);
        Assert.Single(returnedUsers);
        Assert.Equal("testuser", returnedUsers.First().Username);
    }

    [Fact]
    public async Task GetUsers_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.GetUsers()).ReturnsAsync(LogicResult<IList<User>>.NotFound("No users found"));

        // Act
        var result = await controller.GetUsers();

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("No users found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetUser_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var user = new User { Username = "testuser" };
        mockLogic.Setup(l => l.GetUser(It.IsAny<string>())).ReturnsAsync(LogicResult<User>.Ok(user));

        // Act
        var result = await controller.GetUser("valid-user-id");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);
    }

    [Fact]
    public async Task GetUser_Logic_ShouldReturnBadRequest() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.GetUser(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<User>.BadRequest("Invalid user id"));

        // Act
        var result = await controller.GetUser("invalid-user-id");

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid user id", badRequestResult.Value);
    }

    [Fact]
    public async Task GetUser_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetUser("valid-user-id");

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetUser_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.GetUser(It.IsAny<string>())).ReturnsAsync(LogicResult<User>.NotFound("User not found"));

        // Act
        var result = await controller.GetUser("invalid-user-id");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetUserByUsername_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var user = new User { Username = "testuser" };
        mockLogic.Setup(l => l.GetUserByUsername(It.IsAny<string>())).ReturnsAsync(LogicResult<User>.Ok(user));

        // Act
        var result = await controller.GetUserByUsername("valid-username");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);
    }

    [Fact]
    public async Task GetUserByUsername_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.GetUserByUsername(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<User>.NotFound("User not found"));

        // Act
        var result = await controller.GetUserByUsername("invalid-username");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task GetUserByEmail_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var user = new User { Username = "testuser" };
        mockLogic.Setup(l => l.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(LogicResult<User>.Ok(user));

        // Act
        var result = await controller.GetUserByEmail("valid-email@example.com");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("testuser", returnedUser.Username);
    }

    [Fact]
    public async Task GetUserByEmail_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<User>.NotFound("User not found"));

        // Act
        var result = await controller.GetUserByEmail("invalid-email@example.com");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task CreateUser_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var newUser = new User { Username = "newuser" };
        mockLogic.Setup(l => l.CreateUser(It.IsAny<UserCreator>())).ReturnsAsync(LogicResult<User>.Ok(newUser));

        // Act
        var result = await controller.CreateUser(new UserCreator
            { Username = "newuser", Email = "newuser@example.com", Password = "password" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var createdUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("newuser", createdUser.Username);
    }

    [Fact]
    public async Task CreateUser_Logic_ShouldReturnConflict() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.CreateUser(It.IsAny<UserCreator>()))
            .ReturnsAsync(LogicResult<User>.Conflict("User already exists"));

        // Act
        var result = await controller.CreateUser(new UserCreator
            { Username = "existinguser", Email = "existinguser@example.com", Password = "password" });

        // Assert
        var conflictResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.Conflict, conflictResult.StatusCode);
        Assert.Equal("User already exists", conflictResult.Value);
    }

    [Fact]
    public async Task UpdateUser_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var updatedUser = new User { Username = "updateduser" };
        mockLogic.Setup(l => l.UpdateUser(It.IsAny<string>(), It.IsAny<User>()))
            .ReturnsAsync(LogicResult<User>.Ok(updatedUser));

        // Act
        var result =
            await controller.UpdateUser("valid-user-id", new User { Id = Guid.NewGuid(), Username = "updateduser" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedUser = Assert.IsType<User>(okResult.Value);
        Assert.Equal("updateduser", returnedUser.Username);
    }

    [Fact]
    public async Task UpdateUser_Logic_ShouldReturnBadRequest() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdateUser(It.IsAny<string>(), It.IsAny<User>()))
            .ReturnsAsync(LogicResult<User>.BadRequest("Invalid user id"));

        // Act
        var result = await controller.UpdateUser("invalid-user-id",
            new User { Id = Guid.NewGuid(), Username = "updateduser" });

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid user id", badRequestResult.Value);
    }

    [Fact]
    public async Task UpdateUser_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdateUser(It.IsAny<string>(), It.IsAny<User>()))
            .ReturnsAsync(LogicResult<User>.NotFound("User not found"));

        // Act
        var result = await controller.UpdateUser("nonexistent-user-id",
            new User { Id = Guid.NewGuid(), Username = "nonexistentuser" });

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task UpdateUser_Logic_ShouldReturnConflict() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdateUser(It.IsAny<string>(), It.IsAny<User>()))
            .ReturnsAsync(LogicResult<User>.Conflict("Conflict in user update"));

        // Act
        var result = await controller.UpdateUser("conflict-user-id",
            new User { Id = Guid.NewGuid(), Username = "conflictuser" });

        // Assert
        var conflictResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.Conflict, conflictResult.StatusCode);
        Assert.Equal("Conflict in user update", conflictResult.Value);
    }

    [Fact]
    public async Task DeleteUser_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.DeleteUser(It.IsAny<string>())).ReturnsAsync(LogicResult.Ok());

        // Act
        var result = await controller.DeleteUser("valid-user-id");

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task DeleteUser_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.DeleteUser(It.IsAny<string>())).ReturnsAsync(LogicResult.NotFound("User not found"));

        // Act
        var result = await controller.DeleteUser("invalid-user-id");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteUser_Logic_ShouldReturnBadRequest() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.DeleteUser(It.IsAny<string>())).ReturnsAsync(LogicResult.BadRequest("Invalid user id"));

        // Act
        var result = await controller.DeleteUser("invalid-user-id");

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid user id", badRequestResult.Value);
    }

    [Fact]
    public async Task ChangePassword_Logic_ShouldReturnOk() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(LogicResult.Ok());

        // Act
        var result = await controller.ChangePassword("valid-user-id",
            new UserPasswordChange { OldPassword = "oldPassword", NewPassword = "newPassword" });

        // Assert
        var okResult = Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task ChangePassword_Logic_ShouldReturnBadRequest() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(LogicResult.BadRequest("Invalid user id"));

        // Act
        var result = await controller.ChangePassword("invalid-user-id",
            new UserPasswordChange { OldPassword = "oldPassword", NewPassword = "newPassword" });

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid user id", badRequestResult.Value);
    }

    [Fact]
    public async Task ChangePassword_Logic_ShouldReturnNotFound() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(LogicResult.NotFound("User not found"));

        // Act
        var result = await controller.ChangePassword("nonexistent-user-id",
            new UserPasswordChange { OldPassword = "oldPassword", NewPassword = "newPassword" });

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public async Task ChangePassword_Logic_ShouldReturnConflict() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockLogic.Setup(l => l.UpdatePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(LogicResult.Conflict("Old password is not correct"));

        // Act
        var result = await controller.ChangePassword("conflict-user-id",
            new UserPasswordChange { OldPassword = "wrongOldPassword", NewPassword = "newPassword" });

        // Assert
        var conflictResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Conflict, conflictResult.StatusCode);
        Assert.Equal("Old password is not correct", conflictResult.Value);
    }

    [Fact]
    public async Task GetUserByUsername_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetUserByUsername("valid-username");

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetUserByEmail_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetUserByEmail("valid-email@example.com");

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task CreateUser_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.CreateUser(new UserCreator
            { Username = "newuser", Email = "newuser@example.com", Password = "password" });

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task UpdateUser_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result =
            await controller.UpdateUser("valid-user-id", new User { Id = Guid.NewGuid(), Username = "updateduser" });

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task DeleteUser_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.DeleteUser("valid-user-id");

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task ChangePassword_Logic_ShouldReturnUnauthorized() {
        // Arrange
        var (mockLogic, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.ChangePassword("valid-user-id",
            new UserPasswordChange { OldPassword = "oldPassword", NewPassword = "newPassword" });

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
}