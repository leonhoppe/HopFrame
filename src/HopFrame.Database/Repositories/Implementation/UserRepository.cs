using System.Globalization;
using System.Text;
using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace HopFrame.Database.Repositories.Implementation;

internal sealed class UserRepository<TDbContext>(TDbContext context, IGroupRepository groupRepository) : IUserRepository where TDbContext : HopDbContextBase {

    private IIncludableQueryable<User, IList<Token>> IncludeReferences() {
        return context.Users
            .Include(u => u.Permissions)
            .Include(u => u.Tokens);
    }
    
    public async Task<IList<User>> GetUsers() {
        return await IncludeReferences()
            .ToListAsync();
    }

    public async Task<User> GetUser(Guid userId) {
        return await IncludeReferences()
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
    }

    public async Task<User> GetUserByEmail(string email) {
        return await IncludeReferences()
            .Where(u => u.Email == email)
            .SingleOrDefaultAsync();
    }

    public async Task<User> GetUserByUsername(string username) {
        return await IncludeReferences()
            .Where(u => u.Username == username)
            .SingleOrDefaultAsync();
    }

    public async Task<User> AddUser(User user) {
        if (await GetUserByEmail(user.Email) is not null) return null;
        if (await GetUserByUsername(user.Username) is not null) return null;
        
        var entry = new User {
            Id = Guid.NewGuid(),
            Email = user.Email,
            Username = user.Username,
            CreatedAt = DateTime.Now,
            Permissions = user.Permissions ?? new List<Permission>(),
            Tokens = user.Tokens
        };
        entry.Password = EncryptionManager.Hash(user.Password, Encoding.Default.GetBytes(entry.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        
        var defaultGroups = await groupRepository.GetDefaultGroups();
        foreach (var group in defaultGroups) {
            entry.Permissions.Add(new Permission {
                PermissionName = group.Name,
                GrantedAt = DateTime.Now
            });
        }

        await context.Users.AddAsync(entry);
        await context.SaveChangesAsync();
        return entry;
    }

    public async Task UpdateUser(User user) {
        var entry = await IncludeReferences()
            .SingleOrDefaultAsync(entry => entry.Id == user.Id);
        if (entry is null) return;

        // Update the main entity's properties
        entry.Email = user.Email;
        entry.Username = user.Username;

        // Update Permissions
        foreach (var permission in user.Permissions) {
            var existingPermission = entry.Permissions.FirstOrDefault(p => p.Id == permission.Id);
            if (existingPermission != null) {
                // Update existing permission
                context.Entry(existingPermission).CurrentValues.SetValues(permission);
            } else {
                // Add new permission
                entry.Permissions.Add(permission);
            }
        }

        // Remove deleted permissions
        foreach (var permission in entry.Permissions.ToList().Where(permission => user.Permissions.All(p => p.Id != permission.Id))) {
            entry.Permissions.Remove(permission);
            context.Permissions.Remove(permission); // Ensure it gets removed from the database
        }

        // Update Tokens
        foreach (var token in user.Tokens) {
            var existingToken = entry.Tokens.FirstOrDefault(t => t.TokenId == token.TokenId);
            if (existingToken != null) {
                // Update existing token
                context.Entry(existingToken).CurrentValues.SetValues(token);
            } else {
                // Add new token
                entry.Tokens.Add(token);
            }
        }

        // Remove deleted tokens
        foreach (var token in entry.Tokens.ToList().Where(token => user.Tokens.All(t => t.TokenId != token.TokenId))) {
            entry.Tokens.Remove(token);
            context.Tokens.Remove(token); // Ensure it gets removed from the database
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteUser(User user) {
        var entry = await context.Users
            .SingleOrDefaultAsync(entry => entry.Id == user.Id);
        
        if (entry is null) return;

        context.Users.Remove(entry);
        
        await context.SaveChangesAsync();
    }

    public async Task<bool> CheckUserPassword(User user, string password) {
        var hash = EncryptionManager.Hash(password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));

        var entry = await context.Users
            .Where(entry => entry.Id == user.Id)
            .SingleOrDefaultAsync();

        return entry.Password == hash;
    }

    public async Task ChangePassword(User user, string password) {
        var entry = await context.Users
            .Where(entry => entry.Id == user.Id)
            .SingleOrDefaultAsync();
        
        if (entry is null) return;
        
        var hash = EncryptionManager.Hash(password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        entry.Password = hash;
        await context.SaveChangesAsync();
    }
    
}