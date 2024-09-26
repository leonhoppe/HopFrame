using HopFrame.Database.Models;

namespace HopFrame.Security.Services;

/// <summary>
/// permission system:<br/>
/// - "*" -> all rights<br/>
/// - "group.[name]" -> group member<br/>
/// - "[namespace].[name]" -> single permission<br/>
/// - "[namespace].*" -> all permissions in the namespace
/// </summary>
public interface IPermissionService {

    Task<bool> HasPermission(string permission, Guid user);

    Task<IList<PermissionGroup>> GetPermissionGroups();

    Task<PermissionGroup> GetPermissionGroup(string name);

    Task EditPermissionGroup(PermissionGroup group);

    Task<IList<PermissionGroup>> GetUserPermissionGroups(User user);

    Task RemoveGroupFromUser(User user, PermissionGroup group);

    Task<PermissionGroup> CreatePermissionGroup(string name, bool isDefault = false, string description = null);

    Task DeletePermissionGroup(PermissionGroup group);

    Task<Permission> GetPermission(string name, IPermissionOwner owner);

    /// <summary>
    /// permission system:<br/>
    /// - "*" -> all rights<br/>
    /// - "group.[name]" -> group member<br/>
    /// - "[namespace].[name]" -> single permission<br/>
    /// - "[namespace].*" -> all permissions in the namespace
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="permission"></param>
    /// <returns></returns>
    Task AddPermission(IPermissionOwner owner, string permission);

    Task RemovePermission(Permission permission);

    Task<string[]> GetFullPermissions(string user);

}