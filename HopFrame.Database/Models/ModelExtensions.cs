using HopFrame.Database.Models.Entries;

namespace HopFrame.Database.Models;

public static class ModelExtensions {

    /// <summary>
    /// Converts the database model to a friendly user model
    /// </summary>
    /// <param name="entry">the database model</param>
    /// <param name="contextBase">the data source for the permissions and users</param>
    /// <returns></returns>
    public static User ToUserModel(this UserEntry entry, HopDbContextBase contextBase) {
        var user = new User {
            Id = Guid.Parse(entry.Id),
            Username = entry.Username,
            Email = entry.Email,
            CreatedAt = entry.CreatedAt
        };

        user.Permissions = contextBase.Permissions
            .Where(perm => perm.UserId == entry.Id)
            .Select(perm => perm.ToPermissionModel())
            .ToList();

        return user;
    }

    public static Permission ToPermissionModel(this PermissionEntry entry) {
        Guid.TryParse(entry.UserId, out var userId);

        return new Permission {
            Owner = userId,
            PermissionName = entry.PermissionText,
            GrantedAt = entry.GrantedAt,
            Id = entry.RecordId
        };
    }

    public static PermissionGroup ToPermissionGroup(this GroupEntry entry, HopDbContextBase contextBase) {
        var group = new PermissionGroup {
            Name = entry.Name,
            IsDefaultGroup = entry.Default,
            Description = entry.Description,
            CreatedAt = entry.CreatedAt
        };

        group.Permissions = contextBase.Permissions
            .Where(perm => perm.UserId == group.Name)
            .Select(perm => perm.ToPermissionModel())
            .ToList();

        return group;
    }
    
}