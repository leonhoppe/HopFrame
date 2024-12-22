using HopFrame.Database.Models;

namespace HopFrame.Api.Logic;

public interface IGroupLogic {
    Task<LogicResult<IList<PermissionGroup>>> GetGroups();
    Task<LogicResult<IList<PermissionGroup>>> GetDefaultGroups();
    Task<LogicResult<IList<PermissionGroup>>> GetUserGroups(string userId);
    Task<LogicResult<PermissionGroup>> GetGroup(string name);

    Task<LogicResult<PermissionGroup>> CreateGroup(PermissionGroup group);
    Task<LogicResult<PermissionGroup>> UpdateGroup(PermissionGroup group);
    Task<LogicResult> DeleteGroup(string name);
}