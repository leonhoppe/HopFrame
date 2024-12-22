using HopFrame.Api.Models;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Claims;

namespace HopFrame.Api.Logic.Implementation;

internal sealed class UserLogic(IUserRepository users, ITokenContext context) : IUserLogic {
    public async Task<LogicResult<IList<User>>> GetUsers() {
        return LogicResult<IList<User>>.Ok(await users.GetUsers());
    }

    public async Task<LogicResult<User>> GetUser(string id) {
        if (!Guid.TryParse(id, out var userId))
            return LogicResult<User>.BadRequest("Invalid user id");

        var user = await users.GetUser(userId);
        
        if (user is null)
            return LogicResult<User>.NotFound("That user does not exist");

        return LogicResult<User>.Ok(user);
    }

    public async Task<LogicResult<User>> GetUserByUsername(string username) {
        var user = await users.GetUserByUsername(username);
        
        if (user is null)
            return LogicResult<User>.NotFound("That user does not exist");

        return LogicResult<User>.Ok(user);
    }

    public async Task<LogicResult<User>> GetUserByEmail(string email) {
        var user = await users.GetUserByEmail(email);
        
        if (user is null)
            return LogicResult<User>.NotFound("That user does not exist");

        return LogicResult<User>.Ok(user);
    }

    public async Task<LogicResult<User>> CreateUser(UserCreator user) {
        var createdUser = new User {
            Email = user.Email,
            Username = user.Username,
            Password = user.Password,
        };
        createdUser.Permissions = user.Permissions?.Select(p => new Permission {
            GrantedAt = DateTime.Now,
            PermissionName = p,
            User = createdUser
        }).ToList();
        
        var newUser = await users.AddUser(createdUser);

        if (newUser is null)
            return LogicResult<User>.Conflict("That user already exists");

        return LogicResult<User>.Ok(newUser);
    }

    public async Task<LogicResult<User>> UpdateUser(string id, User user) {
        if (!Guid.TryParse(id, out var userId))
            return LogicResult<User>.BadRequest("Invalid user id");
        
        if (user.Id != userId)
            return LogicResult<User>.Conflict("Cannot edit user with different user id");

        if (await users.GetUser(userId) is null)
            return LogicResult<User>.NotFound("That user does not exist");

        await users.UpdateUser(user);
        return LogicResult<User>.Ok(user);
    }

    public async Task<LogicResult> DeleteUser(string id) {
        if (!Guid.TryParse(id, out var userId))
            return LogicResult.BadRequest("Invalid user id");

        var user = await users.GetUser(userId);
        
        if (user is null)
            return LogicResult.NotFound("That user does not exist");

        await users.DeleteUser(user);
        return LogicResult.Ok();
    }

    public async Task<LogicResult> UpdatePassword(string id, string oldPassword, string newPassword) {
        if (!Guid.TryParse(id, out var userId))
            return LogicResult.BadRequest("Invalid user id");

        var user = await users.GetUser(userId);
        
        if (user is null)
            return LogicResult.NotFound("That user does not exist");
        
        if (userId == context.User.Id && !await users.CheckUserPassword(user, oldPassword))
            return LogicResult.Conflict("Old password is not correct");

        await users.ChangePassword(user, newPassword);
        return LogicResult.Ok();
    }
}