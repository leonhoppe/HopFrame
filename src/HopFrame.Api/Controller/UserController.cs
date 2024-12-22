using HopFrame.Api.Logic;
using HopFrame.Api.Models;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HopFrame.Api.Controller;

[ApiController, Route("api/v1/users")]
public class UserController(IOptions<AdminPermissionOptions> permissions, IPermissionRepository perms, ITokenContext context, IUserLogic logic) : ControllerBase {

    private async Task<bool> AuthorizeRequest(string permission) {
        return await perms.HasPermission(context.AccessToken, permission);
    }

    [HttpGet, Authorized]
    public async Task<ActionResult<IList<User>>> GetUsers() {
        if (!await AuthorizeRequest(permissions.Value.Users.Read))
            return Unauthorized();

        return await logic.GetUsers();
    }

    [HttpGet("{userId}"), Authorized]
    public async Task<ActionResult<User>> GetUser(string userId) {
        if (!await AuthorizeRequest(permissions.Value.Users.Read))
            return Unauthorized();

        return await logic.GetUser(userId);
    }

    [HttpGet("username/{username}"), Authorized]
    public async Task<ActionResult<User>> GetUserByUsername(string username) {
        if (!await AuthorizeRequest(permissions.Value.Users.Read))
            return Unauthorized();

        return await logic.GetUserByUsername(username);
    }
    
    [HttpGet("email/{email}"), Authorized]
    public async Task<ActionResult<User>> GetUserByEmail(string email) {
        if (!await AuthorizeRequest(permissions.Value.Users.Read))
            return Unauthorized();

        return await logic.GetUserByEmail(email);
    }

    [HttpPost, Authorized]
    public async Task<ActionResult<User>> CreateUser([FromBody] UserCreator user) {
        if (!await AuthorizeRequest(permissions.Value.Users.Create))
            return Unauthorized();

        return await logic.CreateUser(user);
    }

    [HttpPut("{userId}"), Authorized]
    public async Task<ActionResult<User>> UpdateUser(string userId, [FromBody] User user) {
        if (!await AuthorizeRequest(permissions.Value.Users.Update))
            return Unauthorized();

        return await logic.UpdateUser(userId, user);
    }

    [HttpDelete("{userId}"), Authorized]
    public async Task<ActionResult> DeleteUser(string userId) {
        if (!await AuthorizeRequest(permissions.Value.Users.Delete))
            return Unauthorized();

        return await logic.DeleteUser(userId);
    }

    [HttpPut("{userId}/password"), Authorized]
    public async Task<ActionResult> ChangePassword(string userId, [FromBody] UserPasswordChange passwordChange) {
        if (context.User.Id.ToString() != userId && !await AuthorizeRequest(permissions.Value.Users.Update))
            return Unauthorized();

        return await logic.UpdatePassword(userId, passwordChange.OldPassword, passwordChange.NewPassword);
    }
    
}