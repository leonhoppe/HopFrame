using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Web.Admin;

namespace HopFrame.Web.Repositories;

internal sealed class GroupProvider(IGroupRepository repo) : ModelRepository<PermissionGroup> {
    public override async Task<IEnumerable<PermissionGroup>> ReadAll() {
        return await repo.GetPermissionGroups();
    }

    public override async Task<PermissionGroup> Create(PermissionGroup model) {
        return await repo.CreatePermissionGroup(model);
    }

    public override async Task<PermissionGroup> Update(PermissionGroup model) {
        await repo.EditPermissionGroup(model);
        return model;
    }

    public override Task Delete(PermissionGroup model) {
        return repo.DeletePermissionGroup(model);
    }
}