using HopFrame.Database.Models;

namespace HopFrame.Database.Repositories;

public interface IGroupRepository {
    Task<IList<PermissionGroup>> GetPermissionGroups();

    Task<IList<PermissionGroup>> GetDefaultGroups();

    Task<PermissionGroup> GetPermissionGroup(string name);

    Task EditPermissionGroup(PermissionGroup group);

    Task<PermissionGroup> CreatePermissionGroup(PermissionGroup group);

    Task DeletePermissionGroup(PermissionGroup group);

    internal Task<IList<string>> GetFullGroupPermissions(string group);
}