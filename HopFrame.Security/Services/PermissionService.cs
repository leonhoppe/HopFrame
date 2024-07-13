using HopFrame.Database;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Security.Services;

internal class PermissionService<TDbContext>(TDbContext context, ITokenContext current) : IPermissionService where TDbContext : HopDbContextBase {
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

    private async Task<string[]> GetFullPermissions(string user) {
        var permissions = await context.Permissions
            .Where(perm => perm.UserId == user)
            .Select(perm => perm.PermissionText)
            .ToListAsync();

        var groups = permissions
            .Where(perm => perm.StartsWith("group."))
            .ToList();

        var groupPerms = await context.Permissions
            .Where(perm => groups.Contains(user))
            .Select(perm => perm.PermissionText)
            .ToListAsync();
        
        permissions.AddRange(groupPerms);

        return permissions.ToArray();
    }
}