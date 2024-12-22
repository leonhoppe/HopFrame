using HopFrame.Database.Models;
using HopFrame.Database.Repositories;

namespace HopFrame.Api.Logic.Implementation;

internal sealed class GroupLogic(IGroupRepository groups, IUserRepository users) : IGroupLogic {
    public async Task<LogicResult<IList<PermissionGroup>>> GetGroups() {
        return LogicResult<IList<PermissionGroup>>.Ok(await groups.GetPermissionGroups());
    }

    public async Task<LogicResult<IList<PermissionGroup>>> GetDefaultGroups() {
        return LogicResult<IList<PermissionGroup>>.Ok(await groups.GetDefaultGroups());
    }

    public async Task<LogicResult<IList<PermissionGroup>>> GetUserGroups(string id) {
        if (!Guid.TryParse(id, out var userId))
            return LogicResult<IList<PermissionGroup>>.BadRequest("Invalid user id");

        var user = await users.GetUser(userId);
        
        if (user is null)
            return LogicResult<IList<PermissionGroup>>.NotFound("That user does not exist");

        return LogicResult<IList<PermissionGroup>>.Ok(await groups.GetUserGroups(user));
    }

    public async Task<LogicResult<PermissionGroup>> GetGroup(string name) {
        var group = await groups.GetPermissionGroup(name);

        if (group is null)
            return LogicResult<PermissionGroup>.NotFound("That group does not exist");

        return LogicResult<PermissionGroup>.Ok(group);
    }

    public async Task<LogicResult<PermissionGroup>> CreateGroup(PermissionGroup group) {
        if (group is null)
            return LogicResult<PermissionGroup>.BadRequest("Provide a group");

        if (!group.Name.StartsWith("group."))
            return LogicResult<PermissionGroup>.BadRequest("Group names must start with 'group.'");

        if (await groups.GetPermissionGroup(group.Name) != null)
            return LogicResult<PermissionGroup>.Conflict("That group already exists");
        
        return LogicResult<PermissionGroup>.Ok(await groups.CreatePermissionGroup(group));
    }

    public async Task<LogicResult<PermissionGroup>> UpdateGroup(PermissionGroup group) {
        if (await groups.GetPermissionGroup(group.Name) == null)
            return LogicResult<PermissionGroup>.NotFound("That user does not exist");

        await groups.EditPermissionGroup(group);
        return LogicResult<PermissionGroup>.Ok(group);
    }

    public async Task<LogicResult> DeleteGroup(string name) {
        var group = await groups.GetPermissionGroup(name);

        if (group is null)
            return LogicResult.NotFound("That group does not exist");

        await groups.DeletePermissionGroup(group);
        return LogicResult.Ok();
    }
}