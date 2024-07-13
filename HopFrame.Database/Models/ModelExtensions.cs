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
            .Select(perm => perm.PermissionText)
            .ToList();

        return user;
    }
    
}