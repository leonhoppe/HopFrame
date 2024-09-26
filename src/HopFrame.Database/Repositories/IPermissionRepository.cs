using HopFrame.Database.Models;

namespace HopFrame.Database.Repositories;

public interface IPermissionRepository {
    Task<bool> HasPermission(IPermissionOwner owner, params string[] permissions);
    
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
    Task<Permission> AddPermission(IPermissionOwner owner, string permission);
    
    Task RemovePermission(IPermissionOwner owner, string permission);

    public Task<IList<string>> GetFullPermissions(IPermissionOwner owner);
}