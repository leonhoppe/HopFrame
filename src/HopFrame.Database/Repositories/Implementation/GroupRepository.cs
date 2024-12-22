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

    public Task<IList<PermissionGroup>> GetUserGroups(User user) {
        return Task.FromResult((IList<PermissionGroup>) context.Groups
            .Include(g => g.Permissions)
            .AsEnumerable()
            .Where(g => user.Permissions.Any(p => p.PermissionName == g.Name))
            .ToList());
    }

    public async Task<PermissionGroup> GetPermissionGroup(string name) {
        return await context.Groups
            .Include(g => g.Permissions)
            .Where(g => g.Name == name)
            .SingleOrDefaultAsync();
    }

    public async Task EditPermissionGroup(PermissionGroup group) {
        var orig = await context.Groups
            .Include(g => g.Permissions) // Include related entities
            .SingleOrDefaultAsync(g => g.Name == group.Name);

        if (orig is null) return;

        // Update the main entity's properties
        orig.IsDefaultGroup = group.IsDefaultGroup;
        orig.Description = group.Description;

        // Update the permissions
        foreach (var permission in group.Permissions) {
            var existingPermission = orig.Permissions.FirstOrDefault(p => p.Id == permission.Id);
            if (existingPermission != null) {
                // Update existing permission
                context.Entry(existingPermission).CurrentValues.SetValues(permission);
            } else {
                // Add new permission
                orig.Permissions.Add(permission);
            }
        }

        // Remove deleted permissions
        foreach (var permission in orig.Permissions.ToList().Where(permission => group.Permissions.All(p => p.Id != permission.Id))) {
            orig.Permissions.Remove(permission);
            context.Permissions.Remove(permission); // Ensure it gets removed from the database
        }

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