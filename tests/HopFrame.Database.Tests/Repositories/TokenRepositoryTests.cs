using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Database.Repositories.Implementation;
using HopFrame.Database.Tests.Data;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Tests.Repositories;

public class TokenRepositoryTests {

    private async Task<(DatabaseContext, ITokenRepository)> SetupEnvironment(int count = 5) {
        var context = new DatabaseContext();
        var repo = new TokenRepository<DatabaseContext>(context);

        for (int i = 0; i < count; i++) {
            await context.Tokens.AddAsync(new() {
                Content = Guid.NewGuid(),
                Owner = CreateTestUser(),
                Type = Token.AccessTokenType
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
    public async Task GetToken_Return_CorrectToken() {
        // Arrange
        var (context, repo) = await SetupEnvironment();
        var token = context.Tokens.First();
        
        // Act
        var result = await repo.GetToken(token.Content.ToString());
        
        // Assert
        Assert.Equal(token, result);
    }

    [Fact]
    public async Task CreateToken_Should_CreateOneToken() {
        // Arrange
        var (context, repo) = await SetupEnvironment(0);
        
        // Act
        var result = await repo.CreateToken(Token.AccessTokenType, CreateTestUser());
        
        // Assert
        Assert.NotEmpty(context.Tokens);
        Assert.Equal(result, context.Tokens.First());
    }

    [Fact]
    public async Task DeleteUserTokens_Should_DeleteOnlyUserTokens() {
        // Arrange
        var dummyCount = 5;
        var (context, repo) = await SetupEnvironment(dummyCount);
        var user = CreateTestUser();
        await context.Tokens.AddRangeAsync(new List<Token> {
            new() {
                Content = Guid.NewGuid(),
                Owner = user,
                Type = Token.AccessTokenType
            },
            new() {
                Content = Guid.NewGuid(),
                Owner = user,
                Type = Token.RefreshTokenType
            }
        });
        await context.SaveChangesAsync();
        
        // Act
        await repo.DeleteUserTokens(user);
        
        // Assert
        Assert.Equal(dummyCount, context.Tokens.Count());
        Assert.Empty(context.Tokens
            .Include(t => t.Owner)
            .Where(t => t.Owner == user));
    }
    
}