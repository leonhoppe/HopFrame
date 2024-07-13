namespace HopFrame.Security.Services;

public interface IPermissionService {
    
    /// <summary>
    /// Checks for the user to have the specified permission
    /// Permission system:<br/>
    /// - "*" -> all rights<br/>
    /// - "group.[name]" -> group member<br/>
    /// - "[namespace].[name]" -> single permission<br/>
    /// - "[namespace].*" -> all permissions in the namespace
    /// </summary>
    /// <param name="permission">The permission the user needs</param>
    /// <returns>rather the user has the permission or not</returns>
    Task<bool> HasPermission(string permission);
    
    /// <summary>
    /// Checks if the user has all the specified permissions
    /// </summary>
    /// <param name="permissions">list of the permissions</param>
    /// <returns>rather the user has all the permissions or not</returns>
    Task<bool> HasPermissions(params string[] permissions);
    
    /// <summary>
    /// Checks if the user has any of the specified permissions
    /// </summary>
    /// <param name="permissions">list of the permissions</param>
    /// <returns>rather the user has any permission or not</returns>
    Task<bool> HasAnyPermission(params string[] permissions);

    /// <summary>
    /// Checks for the user to have the specified permission
    /// Permission system:<br/>
    /// - "*" -> all rights<br/>
    /// - "group.[name]" -> group member<br/>
    /// - "[namespace].[name]" -> single permission<br/>
    /// - "[namespace].*" -> all permissions in the namespace
    /// </summary>
    /// <param name="permission">The permission the user needs</param>
    /// <param name="user">The user who gets checked</param>
    /// <returns>rather the user has the permission or not</returns>
    Task<bool> HasPermission(string permission, Guid user);
}