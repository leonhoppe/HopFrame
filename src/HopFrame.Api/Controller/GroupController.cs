using HopFrame.Api.Logic;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HopFrame.Api.Controller;

[ApiController, Route("api/v1/groups")]
public class GroupController(IOptions<AdminPermissionOptions> permissions, IPermissionRepository perms, ITokenContext context, IGroupLogic groups) : ControllerBase {
    
    private async Task<bool> AuthorizeRequest(string permission) {
        return await perms.HasPermission(context.AccessToken, permission);
    }

    [HttpGet, Authorized]
    public async Task<ActionResult<IList<PermissionGroup>>> GetGroups() {
        if (!await AuthorizeRequest(permissions.Value.Groups.Read))
            return Unauthorized();

        return await groups.GetGroups();
    }
    
    [HttpGet("default"), Authorized]
    public async Task<ActionResult<IList<PermissionGroup>>> GetDefaultGroups() {
        if (!await AuthorizeRequest(permissions.Value.Groups.Read))
            return Unauthorized();

        return await groups.GetDefaultGroups();
    }
    
    [HttpGet("user/{userId}"), Authorized]
    public async Task<ActionResult<IList<PermissionGroup>>> GetUserGroups(string userId) {
        if (!await AuthorizeRequest(permissions.Value.Groups.Read))
            return Unauthorized();

        return await groups.GetUserGroups(userId);
    }

    [HttpGet("{name}"), Authorized]
    public async Task<ActionResult<PermissionGroup>> GetGroup(string name) {
        if (!await AuthorizeRequest(permissions.Value.Groups.Read))
            return Unauthorized();

        return await groups.GetGroup(name);
    }

    [HttpPost, Authorized]
    public async Task<ActionResult<PermissionGroup>> CreateGroup([FromBody] PermissionGroup group) {
        if (!await AuthorizeRequest(permissions.Value.Groups.Create))
            return Unauthorized();

        return await groups.CreateGroup(group);
    }

    [HttpPut, Authorized]
    public async Task<ActionResult<PermissionGroup>> UpdateGroup([FromBody] PermissionGroup group) {
        if (!await AuthorizeRequest(permissions.Value.Groups.Update))
            return Unauthorized();

        return await groups.UpdateGroup(group);
    }

    [HttpDelete("{name}"), Authorized]
    public async Task<ActionResult> DeleteGroup(string name) {
        if (!await AuthorizeRequest(permissions.Value.Groups.Delete))
            return Unauthorized();

        return await groups.DeleteGroup(name);
    }
    
}