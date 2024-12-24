using System.Net;
using HopFrame.Api.Controller;
using HopFrame.Api.Logic;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace HopFrame.Tests.Api.Controllers;

public class GroupControllerTests {
    private (Mock<IGroupLogic>, Mock<IOptions<AdminPermissionOptions>>, Mock<IPermissionRepository>, Mock<ITokenContext>
        , GroupController) SetupEnvironment() {
        var mockGroups = new Mock<IGroupLogic>();
        var mockPermissions = new Mock<IOptions<AdminPermissionOptions>>();
        var mockPerms = new Mock<IPermissionRepository>();
        var mockContext = new Mock<ITokenContext>();

        var options = new AdminPermissionOptions();

        mockPermissions.Setup(o => o.Value).Returns(options);
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);

        var controller = new GroupController(mockPermissions.Object, mockPerms.Object, mockContext.Object,
            mockGroups.Object);

        return (mockGroups, mockPermissions, mockPerms, mockContext, controller);
    }

    [Fact]
    public async Task GetGroups_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetGroups();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetGroups_ShouldReturnGroups_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var groups = new List<PermissionGroup> { new PermissionGroup { Name = "testgroup" } };
        mockGroups.Setup(g => g.GetGroups()).ReturnsAsync(LogicResult<IList<PermissionGroup>>.Ok(groups));

        // Act
        var result = await controller.GetGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IList<PermissionGroup>>(okResult.Value);
        Assert.Single(returnedGroups);
        Assert.Equal("testgroup", returnedGroups.First().Name);
    }

    [Fact]
    public async Task GetDefaultGroups_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetDefaultGroups();

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetDefaultGroups_ShouldReturnGroups_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        var groups = new List<PermissionGroup> { new PermissionGroup { Name = "defaultgroup" } };
        mockGroups.Setup(g => g.GetDefaultGroups()).ReturnsAsync(LogicResult<IList<PermissionGroup>>.Ok(groups));

        // Act
        var result = await controller.GetDefaultGroups();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IList<PermissionGroup>>(okResult.Value);
        Assert.Single(returnedGroups);
        Assert.Equal("defaultgroup", returnedGroups.First().Name);
    }

    [Fact]
    public async Task GetUserGroups_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetUserGroups("valid-user-id");

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetUserGroups_ShouldReturnGroups_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
        var groups = new List<PermissionGroup> { new PermissionGroup { Name = "usergroup" } };
        mockGroups.Setup(g => g.GetUserGroups(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<IList<PermissionGroup>>.Ok(groups));

        // Act
        var result = await controller.GetUserGroups("valid-user-id");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroups = Assert.IsAssignableFrom<IList<PermissionGroup>>(okResult.Value);
        Assert.Single(returnedGroups);
        Assert.Equal("usergroup", returnedGroups.First().Name);
    }

    [Fact]
    public async Task GetGroup_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.GetGroup("group-name");

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task GetGroup_ShouldReturnGroup_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
        var group = new PermissionGroup { Name = "group-name" };
        mockGroups.Setup(g => g.GetGroup(It.IsAny<string>())).ReturnsAsync(LogicResult<PermissionGroup>.Ok(group));

        // Act
        var result = await controller.GetGroup("group-name");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedGroup = Assert.IsType<PermissionGroup>(okResult.Value);
        Assert.Equal("group-name", returnedGroup.Name);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.CreateGroup(new PermissionGroup { Name = "newgroup" });

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnGroup_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
        var group = new PermissionGroup { Name = "newgroup" };
        mockGroups.Setup(g => g.CreateGroup(It.IsAny<PermissionGroup>()))
            .ReturnsAsync(LogicResult<PermissionGroup>.Ok(group));

        // Act
        var result = await controller.CreateGroup(new PermissionGroup { Name = "newgroup" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var createdGroup = Assert.IsType<PermissionGroup>(okResult.Value);
        Assert.Equal("newgroup", createdGroup.Name);
    }

    [Fact]
    public async Task UpdateGroup_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.UpdateGroup(new PermissionGroup { Name = "updategroup" });

        // Assert
        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Fact]
    public async Task UpdateGroup_ShouldReturnGroup_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
        var group = new PermissionGroup { Name = "updategroup" };
        mockGroups.Setup(g => g.UpdateGroup(It.IsAny<PermissionGroup>()))
            .ReturnsAsync(LogicResult<PermissionGroup>.Ok(group));

        // Act
        var result = await controller.UpdateGroup(new PermissionGroup { Name = "updategroup" });

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var updatedGroup = Assert.IsType<PermissionGroup>(okResult.Value);
        Assert.Equal("updategroup", updatedGroup.Name);
    }

    [Fact]
    public async Task DeleteGroup_ShouldReturnUnauthorized_WhenUserIsNotAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(false);

        // Act
        var result = await controller.DeleteGroup("group-name");

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task DeleteGroup_ShouldReturnOk_WhenUserIsAuthorized() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockPerms.Setup(p => p.HasPermission(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(true);
        mockGroups.Setup(g => g.DeleteGroup(It.IsAny<string>())).ReturnsAsync(LogicResult.Ok());

        // Act
        var result = await controller.DeleteGroup("group-name");

        // Assert
        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task GetUserGroups_ShouldReturnBadRequest_WhenUserIdIsInvalid() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.GetUserGroups(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<IList<PermissionGroup>>.BadRequest("Invalid user id"));

        // Act
        var result = await controller.GetUserGroups("invalid-user-id");

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Invalid user id", badRequestResult.Value);
    }

    [Fact]
    public async Task GetUserGroups_ShouldReturnNotFound_WhenUserDoesNotExist() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.GetUserGroups(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<IList<PermissionGroup>>.NotFound("That user does not exist"));

        // Act
        var result = await controller.GetUserGroups("nonexistent-user-id");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("That user does not exist", notFoundResult.Value);
    }

    [Fact]
    public async Task GetGroup_ShouldReturnNotFound_WhenGroupDoesNotExist() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.GetGroup(It.IsAny<string>()))
            .ReturnsAsync(LogicResult<PermissionGroup>.NotFound("That group does not exist"));

        // Act
        var result = await controller.GetGroup("nonexistent-group-name");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("That group does not exist", notFoundResult.Value);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnBadRequest_WhenGroupIsNull() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.CreateGroup(It.IsAny<PermissionGroup>()))
            .ReturnsAsync(LogicResult<PermissionGroup>.BadRequest("Provide a group"));

        // Act
        var result = await controller.CreateGroup(null);

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Provide a group", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnBadRequest_WhenGroupNameIsInvalid() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.CreateGroup(It.IsAny<PermissionGroup>()))
            .ReturnsAsync(LogicResult<PermissionGroup>.BadRequest("Group names must start with 'group.'"));

        // Act
        var result = await controller.CreateGroup(new PermissionGroup { Name = "invalidgroupname" });

        // Assert
        var badRequestResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        Assert.Equal("Group names must start with 'group.'", badRequestResult.Value);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnConflict_WhenGroupAlreadyExists() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.CreateGroup(It.IsAny<PermissionGroup>()))
            .ReturnsAsync(LogicResult<PermissionGroup>.Conflict("That group already exists"));

        // Act
        var result = await controller.CreateGroup(new PermissionGroup { Name = "group.exists" });

        // Assert
        var conflictResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.Conflict, conflictResult.StatusCode);
        Assert.Equal("That group already exists", conflictResult.Value);
    }
    
    [Fact]
    public async Task UpdateGroup_ShouldReturnNotFound_WhenGroupDoesNotExist() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.UpdateGroup(It.IsAny<PermissionGroup>())).ReturnsAsync(LogicResult<PermissionGroup>.NotFound("That group does not exist"));

        // Act
        var result = await controller.UpdateGroup(new PermissionGroup { Name = "nonexistent-group-name" });

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("That group does not exist", notFoundResult.Value);
    }

    [Fact]
    public async Task DeleteGroup_ShouldReturnNotFound_WhenGroupDoesNotExist() {
        // Arrange
        var (mockGroups, mockPermissions, mockPerms, mockContext, controller) = SetupEnvironment();
        mockGroups.Setup(g => g.DeleteGroup(It.IsAny<string>())).ReturnsAsync(LogicResult.NotFound("That group does not exist"));

        // Act
        var result = await controller.DeleteGroup("nonexistent-group-name");

        // Assert
        var notFoundResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        Assert.Equal("That group does not exist", notFoundResult.Value);
    }
}