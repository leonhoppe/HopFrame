namespace HopFrame.Security.Authorization;

public static class PermissionValidator {
    
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