using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Repositories.Implementation;

internal sealed class PermissionRepository<TDbContext>(TDbContext context, IGroupRepository groupRepository) : IPermissionRepository where TDbContext : HopDbContextBase {
    public async Task<bool> HasPermission(IPermissionOwner owner, params string[] permissions) {
        var perms = (await GetFullPermissions(owner)).ToArray();

        foreach (var permission in permissions) {
            if (!PermissionValidator.IncludesPermission(permission, perms)) return false;
        }

        return true;
    }

    public async Task<Permission> AddPermission(IPermissionOwner owner, string permission) {
        var entry = new Permission {
            GrantedAt = DateTime.Now,
            PermissionName = permission
        };

        if (owner is User user) {
            entry.User = user;
        }else if (owner is PermissionGroup group) {
            entry.Group = group;
        }

        await context.Permissions.AddAsync(entry);
        await context.SaveChangesAsync();
        return entry;
    }

    public async Task RemovePermission(IPermissionOwner owner, string permission) {
        Permission entry = null;
        
        if (owner is User user) {
            entry = await context.Permissions
                .Include(p => p.User)
                .Where(p => p.User != null)
                .Where(p => p.User.Id == user.Id)
                .Where(p => p.PermissionName == permission)
                .SingleOrDefaultAsync();
        }else if (owner is PermissionGroup group) {
            entry = await context.Permissions
                .Include(p => p.Group)
                .Where(p => p.Group != null)
                .Where(p =>p.Group.Name == group.Name)
                .Where(p => p.PermissionName == permission)
                .SingleOrDefaultAsync();
        }

        if (entry is not null) {
            context.Permissions.Remove(entry);
            await context.SaveChangesAsync();
        }
    }
    
    public async Task<IList<string>> GetFullPermissions(IPermissionOwner owner) {
        var permissions = new List<string>();
        
        if (owner is User user) {
            var perms = await context.Permissions
                .Include(p => p.User)
                .Where(p => p.User != null)
                .Where(p => p.User.Id == user.Id)
                .ToListAsync();
            
            permissions.AddRange(perms.Select(p => p.PermissionName));
        }else if (owner is PermissionGroup group) {
            var perms = await context.Permissions
                .Include(p => p.Group)
                .Where(p => p.Group != null)
                .Where(p =>p.Group.Name == group.Name)
                .ToListAsync();
            
            permissions.AddRange(perms.Select(p => p.PermissionName));
        }

        var groups = permissions
            .Where(p => p.StartsWith("group."))
            .ToList();
        foreach (var group in groups) {
            permissions.AddRange(await groupRepository.GetFullGroupPermissions(group));
        }

        return permissions;
    }
}