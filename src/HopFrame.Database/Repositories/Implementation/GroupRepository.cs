using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Repositories.Implementation;

internal sealed class GroupRepository<TDbContext>(TDbContext context) : IGroupRepository where TDbContext : HopDbContextBase {
    public async Task<IList<PermissionGroup>> GetPermissionGroups() {
        return await context.Groups
            .Include(g => g.Permissions)
            .ToListAsync();
    }

    public async Task<IList<PermissionGroup>> GetDefaultGroups() {
        return await context.Groups
            .Include(g => g.Permissions)
            .Where(g => g.IsDefaultGroup)
            .ToListAsync();
    }

    public async Task<PermissionGroup> GetPermissionGroup(string name) {
        return await context.Groups
            .Include(g => g.Permissions)
            .Where(g => g.Name == name)
            .SingleOrDefaultAsync();
    }

    public async Task EditPermissionGroup(PermissionGroup group) {
        var orig = await context.Groups.SingleOrDefaultAsync(g => g.Name == group.Name);
        
        if (orig is null) return;

        var entity = context.Groups.Update(orig);

        entity.Entity.IsDefaultGroup = group.IsDefaultGroup;
        entity.Entity.Description = group.Description;
        entity.Entity.Permissions = group.Permissions;

        await context.SaveChangesAsync();
    }

    public async Task<PermissionGroup> CreatePermissionGroup(PermissionGroup group) {
        group.CreatedAt = DateTime.Now;
        await context.Groups.AddAsync(group);
        await context.SaveChangesAsync();
        return group;
    }

    public async Task DeletePermissionGroup(PermissionGroup group) {
        context.Groups.Remove(group);
        await context.SaveChangesAsync();
    }

    public async Task<IList<string>> GetFullGroupPermissions(string group) {
        var permissions = await context.Permissions
            .Include(p => p.Group)
            .Where(p => p.Group != null)
            .Where(p => p.Group.Name == group)
            .Select(p => p.PermissionName)
            .ToListAsync();

        var groups = permissions
            .Where(p => p.StartsWith("group."))
            .ToList();
        
        foreach (var subgroup in groups) {
            permissions.AddRange(await GetFullGroupPermissions(subgroup));
        }

        return permissions;
    }
}