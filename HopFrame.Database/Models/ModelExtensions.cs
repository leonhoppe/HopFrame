using HopFrame.Database.Models.Entries;

namespace HopFrame.Database.Models;

public static class ModelExtensions {

    public static User ToUserModel(this UserEntry entry, HopDbContextBase contextBase) {
        var user = new User {
            Id = Guid.Parse(entry.Id),
            Username = entry.Username,
            Email = entry.Email,
            CreatedAt = entry.CreatedAt
        };

        user.Permissions = contextBase.Permissions
            .Where(perm => perm.UserId == entry.Id)
            .Select(perm => perm.PermissionText)
            .ToList();

        return user;
    }
    
}