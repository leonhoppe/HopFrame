using HopFrame.Database;
using HopFrame.Database.Models;
using HopFrame.Database.Models.Entries;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Security.Services.Implementation;

internal sealed class PermissionService<TDbContext>(TDbContext context, ITokenContext current) : IPermissionService where TDbContext : HopDbContextBase {
    public async Task<bool> HasPermission(string permission) {
        return await HasPermission(permission, current.User.Id);
    }

    public async Task<bool> HasPermissions(params string[] permissions) {
        var user = current.User.Id.ToString();
        var perms = await GetFullPermissions(user);
        
        foreach (var permission in permissions) {
            if (!PermissionValidator.IncludesPermission(permission, perms)) return false;
        }

        return true;
    }

    public async Task<bool> HasAnyPermission(params string[] permissions) {
        var user = current.User.Id.ToString();
        var perms = await GetFullPermissions(user);
        
        foreach (var permission in permissions) {
            if (PermissionValidator.IncludesPermission(permission, perms)) return true;
        }

        return false;
    }

    public async Task<bool> HasPermission(string permission, Guid user) {
        var permissions = await GetFullPermissions(user.ToString());

        return PermissionValidator.IncludesPermission(permission, permissions);
    }

    public async Task<IList<PermissionGroup>> GetPermissionGroups() {
        return await context.Groups
            .Select(group => group.ToPermissionGroup(context))
            .ToListAsync();
    }

    public Task<PermissionGroup> GetPermissionGroup(string name) {
        return context.Groups
            .Where(group => group.Name == name)
            .Select(group => group.ToPermissionGroup(context))
            .SingleOrDefaultAsync();
    }

    public async Task<IList<PermissionGroup>> GetUserPermissionGroups(User user) {
        var groups = await context.Groups.ToListAsync();
        var perms = await GetFullPermissions(user.Id.ToString());

        return groups
            .Where(group => perms.Contains(group.Name))
            .Select(group => group.ToPermissionGroup(context))
            .ToList();
    }

    public async Task RemoveGroupFromUser(User user, PermissionGroup group) {
        var entry = await context.Permissions
            .Where(perm => perm.PermissionText == group.Name && perm.UserId == user.Id.ToString())
            .SingleOrDefaultAsync();
        
        if (entry is null) return;

        context.Permissions.Remove(entry);
        await context.SaveChangesAsync();
    }

    public async Task CreatePermissionGroup(string name, bool isDefault = false, string description = null) {
        var group = new GroupEntry {
            Name = name,
            Description = description,
            Default = isDefault,
            CreatedAt = DateTime.Now
        };

        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();
    }

    public async Task DeletePermissionGroup(PermissionGroup group) {
        var entry = await context.Groups.SingleOrDefaultAsync(entry => entry.Name == group.Name);
        context.Groups.Remove(entry);
        await context.SaveChangesAsync();
    }

    public async Task<Permission> GetPermission(string name, IPermissionOwner owner) {
        var ownerId = (owner is User user) ? user.Id.ToString() : ((PermissionGroup)owner).Name;

        return await context.Permissions
            .Where(perm => perm.PermissionText == name && perm.UserId == ownerId)
            .Select(perm => perm.ToPermissionModel())
            .SingleOrDefaultAsync();
    }

    public async Task AddPermission(IPermissionOwner owner, string permission) {
        var userId = owner is User user ? user.Id.ToString() : (owner as PermissionGroup)?.Name;

        await context.Permissions.AddAsync(new PermissionEntry {
            UserId = userId,
            PermissionText = permission,
            GrantedAt = DateTime.Now
        });
        await context.SaveChangesAsync();
    }

    public async Task RemovePermission(Permission permission) {
        var entry = await context.Permissions.SingleOrDefaultAsync(entry => entry.RecordId == permission.Id);
        context.Permissions.Remove(entry);
        await context.SaveChangesAsync();
    }

    public async Task<string[]> GetFullPermissions(string user) {
        var permissions = await context.Permissions
            .Where(perm => perm.UserId == user)
            .Select(perm => perm.PermissionText)
            .ToListAsync();

        var groups = permissions
            .Where(perm => perm.StartsWith("group."))
            .ToList();

        var groupPerms = new List<string>();
        foreach (var group in groups) {
            var perms = await GetFullPermissions(group);
            groupPerms.AddRange(perms);
        }
        
        permissions.AddRange(groupPerms);

        return permissions.ToArray();
    }
}