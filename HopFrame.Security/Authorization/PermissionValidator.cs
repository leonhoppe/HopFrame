namespace HopFrame.Security.Authorization;

public static class PermissionValidator {
    
    public static bool IncludesPermission(string permission, string[] permissions) {
        var permLow = permission.ToLower();
        var permsLow = permissions.Select(perm => perm.ToLower()).ToArray();
        
        if (permsLow.Any(perm => perm == permLow || perm == "*")) return true;
        
        foreach (var perm in permsLow) {
            if (!perm.EndsWith(".*")) continue;

            var permissionGroup = perm.Replace(".*", "");
            if (permLow.StartsWith(permissionGroup)) return true;
        }

        return false;
    }
    
}