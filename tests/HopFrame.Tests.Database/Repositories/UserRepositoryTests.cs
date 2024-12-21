using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Database.Repositories.Implementation;
using HopFrame.Tests.Database.Data;

namespace HopFrame.Tests.Database.Repositories;

public class UserRepositoryTests {

    private async Task<(DatabaseContext, IUserRepository)> SetupEnvironment(int count = 5) {
        var context = new DatabaseContext();
        var repo = new UserRepository<DatabaseContext>(context, new GroupRepository<DatabaseContext>(context));

        for (int i = 0; i < count; i++) {
            await context.Users.AddAsync(CreateTestUser());
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
    public async Task GetUsers_Returns_AllUsers() {
        // Arrange
        var count = 5;
        var (context, repo) = await SetupEnvironment(count);
        
        // Act
        var users = await repo.GetUsers();
        
        // Assert
        Assert.NotNull(users);
        Assert.Equal(count, users.Count);
    }

    [Fact]
    public async Task GetUser_Returns_SingleUser() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var guid = context.Users.First().Id;
        
        // Act
        var user = await repo.GetUser(guid);
        
        // Assert
        Assert.NotNull(user);
        Assert.Equal(guid, user.Id);
    }

    [Fact]
    public async Task GetUserByMail_Returns_SingleUser() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = CreateTestUser();
        user.Email = "test@example.com";
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = await repo.GetUserByEmail("test@example.com");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }
    
    [Fact]
    public async Task GetUserByUsername_Returns_SingleUser() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = CreateTestUser();
        user.Username = "test.user";
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        
        // Act
        var result = await repo.GetUserByUsername("test.user");
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(user, result);
    }

    [Fact]
    public async Task AddUser_Returns_NewUser() {
        // Arrange
        var count = 5;
        var (context, repo) = await SetupEnvironment(count);
        
        // Act
        var user = await repo.AddUser(new User {
            Username = "test.user",
            Email = "test@example.com",
            Password = "changeme"
        });
        
        // Assert
        Assert.NotNull(user);
        Assert.Equal(count + 1, context.Users.Count());
    }

    [Fact]
    public async Task UpdateUser_Should_UpdateAUser() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var user = context.Users.First();
        
        // Act
        user.Username = "test.user";
        await repo.UpdateUser(user);
        
        // Assert
        var result = context.Users.SingleOrDefault(u => u.Username == "test.user");
        Assert.NotNull(result);
    }

    [Fact]
    public async Task DeleteUser_Should_DeleteSingleUser() {
        // Arrange
        var count = 5;
        var (context, repo) = await SetupEnvironment(count);
        var user = context.Users.First();
        
        // Act
        await repo.DeleteUser(user);
        
        // Assert
        Assert.Equal(count - 1, context.Users.Count());
        Assert.DoesNotContain(user, context.Users);
    }

    [Fact]
    public async Task CheckUserPassword_Returns_True() {
        // Arrange
        var (context, repo) = await SetupEnvironment(0);
        var user = CreateTestUser();
        user.Password = "changeme";
        user = await repo.AddUser(user);
        
        // Act
        var result = await repo.CheckUserPassword(user, "changeme");
        
        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task CheckUserPassword_Returns_False() {
        // Arrange
        var (context, repo) = await SetupEnvironment(0);
        var user = CreateTestUser();
        user.Password = "changeme";
        user = await repo.AddUser(user);
        
        // Act
        var result = await repo.CheckUserPassword(user, "dontchangeme");
        
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ChangePassword_Should_ChangeUserPassword() {
        // Arrange
        var (context, repo) = await SetupEnvironment(0);
        var user = CreateTestUser();
        user.Password = "changeme";
        user = await repo.AddUser(user);
        
        // Act
        await repo.ChangePassword(user, "changedme");
        
        // Assert
        var result = await repo.CheckUserPassword(user, "changedme");
        Assert.True(result);
    }
    
}