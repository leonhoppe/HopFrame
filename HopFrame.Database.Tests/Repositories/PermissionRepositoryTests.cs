using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Database.Repositories.Implementation;
using HopFrame.Database.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Tests.Repositories;

public class PermissionRepositoryTests {

    private async Task<(DatabaseContext, IPermissionRepository)> SetupEnvironment(int count = 5) {
        var context = new DatabaseContext();
        var repo = new PermissionRepository<DatabaseContext>(context, new GroupRepository<DatabaseContext>(context));

        for (int i = 0; i < count; i++) {
            await context.Permissions.AddAsync(new () {
                PermissionName = Guid.NewGuid().ToString(),
                User = CreateTestUser()
            });
        }
        await context.SaveChangesAsync();

        return (context, repo);
    }

    private User CreateTestUser() => new () {
        Username = "",
        Email = "",
        Password = ""
    };

    [Fact]
    public async Task HasPermission_Returns_True() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = CreateTestUser();
        await context.Permissions.AddAsync(new() {
            PermissionName = "*",
            User = user
        });
        await context.SaveChangesAsync();
        
        // Act
        var hasPermission = await repo.HasPermission(user, "*");
        
        // Assert
        Assert.True(hasPermission);
    }

    [Fact]
    public async Task AddPermission_Should_AddOnePermission() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = CreateTestUser();
        
        // Act
        var result = await repo.AddPermission(user, "test.permission");
        
        // Assert
        Assert.NotNull(result);
        Assert.Single(context.Permissions
            .Include(p => p.User)
            .Where(p => p.User.Id == user.Id && p.PermissionName == "test.permission"));
    }

    [Fact]
    public async Task RemovePermission_Should_RemoveOnePermission() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = CreateTestUser();
        await context.Permissions.AddAsync(new() {
            PermissionName = "test.permission",
            User = user
        });
        await context.SaveChangesAsync();
        
        // Act
        await repo.RemovePermission(user, "test.permission");
        
        // Assert
        Assert.Empty(context.Permissions
            .Include(p => p.User)
            .Where(p => p.User.Id == user.Id && p.PermissionName == "test.permission"));
    }

    [Fact]
    public async Task GetFullPermissions_Return_AllPermissions_Including_Inherited() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = CreateTestUser();
        var group = new PermissionGroup {
            Name = "group.admin"
        };
        await context.Permissions.AddRangeAsync(new List<Permission> {
            new() {
                PermissionName = "test.permission.inherited",
                Group = group
            },
            new() {
                PermissionName = "test.permission",
                User = user
            },
            new() {
                PermissionName = "group.admin",
                User = user
            }
        });
        await context.SaveChangesAsync();

        // Act
        var perms = await repo.GetFullPermissions(user);
        
        // Assert
        Assert.NotNull(perms);
        Assert.Equal(3, perms.Count);
    }
    
}