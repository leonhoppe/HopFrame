namespace HopFrame.Security.Authorization;

internal static class PermissionValidator {

    /// <summary>
    /// Checks for the user to have the specified permission<br/>
    /// Permission system:<br/>
    /// - "*" -> all rights<br/>
    /// - "group.[name]" -> group member<br/>
    /// - "[namespace].[name]" -> single permission<br/>
    /// - "[namespace].*" -> all permissions in the namespace
    /// </summary>
    /// <param name="permission">The permission the user needs</param>
    /// <param name="permissions">All the permissions the user has (includes group permissions)</param>
    /// <returns></returns>
    public static bool IncludesPermission(string permission, string[] permissions) {
        if (permission == "*") return true;
        if (permissions.Contains(permission)) return true;
        
        foreach (var perm in permissions) {
            if (!perm.EndsWith(".*")) continue;

            var permissionGroup = perm.Replace(".*", "");
            if (permission.StartsWith(permissionGroup)) return true;
        }

        return false;
    }
    
}