using HopFrame.Api.Models;
using HopFrame.Database.Models;

namespace HopFrame.Api.Logic;

public interface IUserLogic {
    Task<LogicResult<IList<User>>> GetUsers();
    Task<LogicResult<User>> GetUser(string id);
    Task<LogicResult<User>> GetUserByUsername(string username);
    Task<LogicResult<User>> GetUserByEmail(string email);

    Task<LogicResult<User>> CreateUser(UserCreator user);
    Task<LogicResult<User>> UpdateUser(string id, User user);
    Task<LogicResult> DeleteUser(string id);
    Task<LogicResult> UpdatePassword(string id, string oldPassword, string newPassword);
}