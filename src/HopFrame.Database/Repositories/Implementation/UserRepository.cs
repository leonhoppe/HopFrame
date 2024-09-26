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
            .ThenInclude(p => p.Group)
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
            Permissions = user.Permissions,
            Tokens = user.Tokens
        };
        entry.Password = EncryptionManager.Hash(user.Password, Encoding.Default.GetBytes(entry.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        
        var defaultGroups = await groupRepository.GetDefaultGroups();
        foreach (var group in defaultGroups) {
            entry.Permissions.Add(new Permission {
                PermissionName = group.Name,
                //TODO: Check if user needs to be set
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

        entry.Email = user.Email;
        entry.Username = user.Username;
        entry.Permissions = user.Permissions;
        entry.Tokens = user.Tokens;

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