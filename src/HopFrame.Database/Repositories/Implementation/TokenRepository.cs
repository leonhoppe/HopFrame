using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Repositories.Implementation;

internal sealed class TokenRepository<TDbContext>(TDbContext context) : ITokenRepository where TDbContext : HopDbContextBase {
    
    public async Task<Token> GetToken(string content) {
        var success = Guid.TryParse(content, out Guid guid);
        if (!success) return null;
        
        return await context.Tokens
            .Include(t => t.Owner)
            .Where(t => t.TokenId == guid)
            .SingleOrDefaultAsync();
    }

    public async Task<Token> CreateToken(int type, User owner) {
        var token = new Token {
            CreatedAt = DateTime.Now,
            TokenId = Guid.NewGuid(),
            Type = type,
            Owner = owner
        };

        await context.Tokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async Task DeleteUserTokens(User owner, bool includeApiTokens = false) {
        var tokens = await context.Tokens
            .Include(t => t.Owner)
            .Where(t => t.Owner.Id == owner.Id)
            .ToListAsync();

        if (!includeApiTokens)
            tokens = tokens
                .Where(t => t.Type != Token.ApiTokenType)
                .ToList();
        
        context.Tokens.RemoveRange(tokens);
        await context.SaveChangesAsync();
    }

    public async Task DeleteToken(Token token) {
        context.Tokens.Remove(token);
        await context.SaveChangesAsync();
    }

    public async Task<Token> CreateApiToken(User owner, DateTime expirationDate) {
        var token = new Token {
            CreatedAt = expirationDate,
            TokenId = Guid.NewGuid(),
            Type = Token.ApiTokenType,
            Owner = owner
        };

        await context.Tokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }
}