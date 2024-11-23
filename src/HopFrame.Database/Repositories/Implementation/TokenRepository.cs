using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Repositories.Implementation;

internal sealed class TokenRepository<TDbContext>(TDbContext context) : ITokenRepository where TDbContext : HopDbContextBase {
    
    public async Task<Token> GetToken(string content) {
        var success = Guid.TryParse(content, out Guid guid);
        if (!success) return null;
        
        return await context.Tokens
            .Include(t => t.Owner)
            .Where(t => t.Content == guid)
            .SingleOrDefaultAsync();
    }

    public async Task<Token> CreateToken(int type, User owner) {
        var token = new Token {
            CreatedAt = DateTime.Now,
            Content = Guid.NewGuid(),
            Type = type,
            Owner = owner
        };

        await context.Tokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token;
    }

    public async Task DeleteUserTokens(User owner) {
        var tokens = await context.Tokens
            .Include(t => t.Owner)
            .Where(t => t.Owner.Id == owner.Id)
            .ToListAsync();
        
        context.Tokens.RemoveRange(tokens);
        await context.SaveChangesAsync();
    }
}