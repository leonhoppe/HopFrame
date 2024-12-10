using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Database.Repositories.Implementation;
using HopFrame.Tests.Database.Data;

namespace HopFrame.Tests.Database.Repositories;

public class GroupRepositoryTests {

    private async Task<(DatabaseContext, IGroupRepository)> SetupEnvironment(int count = 5) {
        var context = new DatabaseContext();
        var repo = new GroupRepository<DatabaseContext>(context);
        
        for (int i = 0; i < count; i++) {
            await context.Groups.AddAsync(new() {
                Name = Guid.NewGuid().ToString()
            });
        }
        await context.SaveChangesAsync();

        return (context, repo);
    }

    [Fact]
    public async Task GetPermissionGroups_Returns_AllPermissionGroups() {
        // Arrange
        var count = 5;
        var (context, repo) = await SetupEnvironment(count);
        
        // Act
        var groups = await repo.GetPermissionGroups();
        
        // Assert
        Assert.Equal(count, groups.Count);
    }

    [Fact]
    public async Task GetDefaultGroups_Returns_OnlyDefaultGroups() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        await context.Groups.AddAsync(new () {
            Name = Guid.NewGuid().ToString(),
            IsDefaultGroup = true
        });
        await context.SaveChangesAsync();
        
        // Act
        var groups = await repo.GetDefaultGroups();
        
        // Assert
        Assert.Single(groups);
        Assert.True(groups[0].IsDefaultGroup);
    }

    [Fact]
    public async Task GetUserGroups_Returns_OnlyUserGroups() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        await context.Groups.AddAsync(new () {
            Name = "group.user_should_have_these",
        });
        var user = new User {
            Id = Guid.NewGuid(),
            Email = "",
            Username = "",
            Password = ""
        };
        user.Permissions = new() {
            new() {
                User = user,
                PermissionName = "group.user_should_have_these"
            }
        };
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Act
        var groups = await repo.GetUserGroups(user);
        
        // Assert
        Assert.Single(groups);
        Assert.Equal("group.user_should_have_these", groups[0].Name);
    }

    [Fact]
    public async Task GetPermissionGroup_Returns_OnlyOnePermissionGroup() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        await context.Groups.AddAsync(new () {
            Name = "group.return"
        });
        await context.SaveChangesAsync();
        
        // Act
        var group = await repo.GetPermissionGroup("group.return");
        
        // Assert
        Assert.NotNull(group);
        Assert.Equal("group.return", group.Name);
    }

    [Fact]
    public async Task EditPermissionGroup_Should_EditOnePermissionGroup() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var groupToEdit = new PermissionGroup {
            Name = "group.edit"
        };
        await context.Groups.AddAsync(groupToEdit);
        await context.SaveChangesAsync();
        
        // Act
        groupToEdit.Description = "This description was edited";
        await repo.EditPermissionGroup(groupToEdit);
        
        // Assert
        var group = context.Groups.SingleOrDefault(g => g.Name == "group.edit");
        Assert.NotNull(group);
        Assert.Equal("This description was edited", group.Description);
    }

    [Fact]
    public async Task CreatePermissionGroup_Should_AddOnePermissionGroup() {
        // Arrange
        var (context, repo) = await SetupEnvironment(0);
        var group = new PermissionGroup {
            Name = "group",
            Description = "Group to add"
        };

        // Act
        var result = await repo.CreatePermissionGroup(group);
        
        // Assert
        Assert.Equal(group, result);
        Assert.Single(context.Groups);
    }

    [Fact]
    public async Task DeletePermissionGroup_Should_DeleteOnePermissionGroup() {
        // Arrange
        var count = 5;
        var (context, repo) = await SetupEnvironment(count);
        
        // Act
        await repo.DeletePermissionGroup(context.Groups.First());
        
        // Assert
        Assert.Equal(count - 1, context.Groups.Count());
    }
    
}