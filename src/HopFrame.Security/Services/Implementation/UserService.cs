using System.Globalization;
using System.Text;
using HopFrame.Database;
using HopFrame.Database.Models;
using HopFrame.Database.Models.Entries;
using HopFrame.Security.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Security.Services.Implementation;

internal sealed class UserService<TDbContext>(TDbContext context) : IUserService where TDbContext : HopDbContextBase {
    public async Task<IList<User>> GetUsers() {
        return await context.Users
            .Select(user => user.ToUserModel(context))
            .ToListAsync();
    }

    public Task<User> GetUser(Guid userId) {
        var id = userId.ToString();
        
        return context.Users
            .Where(user => user.Id == id)
            .Select(user => user.ToUserModel(context))
            .SingleOrDefaultAsync();
    }

    public Task<User> GetUserByEmail(string email) {
        return context.Users
            .Where(user => user.Email == email)
            .Select(user => user.ToUserModel(context))
            .SingleOrDefaultAsync();
    }

    public Task<User> GetUserByUsername(string username) {
        return context.Users
            .Where(user => user.Username == username)
            .Select(user => user.ToUserModel(context))
            .SingleOrDefaultAsync();
    }

    public async Task<User> AddUser(UserRegister user) {
        if (await GetUserByEmail(user.Email) is not null) return null;
        if (await GetUserByUsername(user.Username) is not null) return null;
        
        var entry = new UserEntry {
            Id = Guid.NewGuid().ToString(),
            Email = user.Email,
            Username = user.Username,
            CreatedAt = DateTime.Now
        };
        entry.Password = EncryptionManager.Hash(user.Password, Encoding.Default.GetBytes(entry.CreatedAt.ToString(CultureInfo.InvariantCulture)));

        await context.Users.AddAsync(entry);
        
        var defaultGroups = await context.Groups
            .Where(group => group.Default)
            .Select(group => "group." + group.Name)
            .ToListAsync();

        await context.Permissions.AddRangeAsync(defaultGroups.Select(group => new PermissionEntry {
            GrantedAt = DateTime.Now,
            PermissionText = group,
            UserId = entry.Id
        }));
        
        await context.SaveChangesAsync();
        return entry.ToUserModel(context);
    }

    public async Task UpdateUser(User user) {
        var id = user.Id.ToString();
        var entry = await context.Users
            .SingleOrDefaultAsync(entry => entry.Id == id);
        if (entry is null) return;

        entry.Email = user.Email;
        entry.Username = user.Username;

        await context.SaveChangesAsync();
    }

    public async Task DeleteUser(User user) {
        var id = user.Id.ToString();
        var entry = await context.Users
            .SingleOrDefaultAsync(entry => entry.Id == id);
        
        if (entry is null) return;

        context.Users.Remove(entry);

        var userTokens = await context.Tokens
            .Where(token => token.UserId == id)
            .ToArrayAsync();
        context.Tokens.RemoveRange(userTokens);

        var userPermissions = await context.Permissions
            .Where(perm => perm.UserId == id)
            .ToArrayAsync();
        context.Permissions.RemoveRange(userPermissions);

        context.OnUserDelete(entry);
        
        await context.SaveChangesAsync();
    }

    public async Task<bool> CheckUserPassword(User user, string password) {
        var id = user.Id.ToString();
        var hash = EncryptionManager.Hash(password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));

        var entry = await context.Users
            .Where(entry => entry.Id == id)
            .SingleOrDefaultAsync();

        return entry.Password == hash;
    }

    public async Task ChangePassword(User user, string password) {
        var entry = await context.Users
            .Where(entry => entry.Id == user.Id.ToString())
            .SingleOrDefaultAsync();
        
        if (entry is null) return;
        
        var hash = EncryptionManager.Hash(password, Encoding.Default.GetBytes(user.CreatedAt.ToString(CultureInfo.InvariantCulture)));
        entry.Password = hash;
        await context.SaveChangesAsync();
    }
}