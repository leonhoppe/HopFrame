using HopFrame.Database.Models;

namespace HopFrame.Security.Services;

public interface IPermissionService {

    Task<bool> HasPermission(string permission, Guid user);

    Task<PermissionGroup> GetPermissionGroup(string name);

    Task CreatePermissionGroup(string name, bool isDefault = false, string description = null);

    Task DeletePermissionGroup(PermissionGroup group);

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

    Task DeletePermission(Permission permission);

    internal Task<string[]> GetFullPermissions(string user);

}