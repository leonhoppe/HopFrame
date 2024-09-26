using HopFrame.Api.Models;
using HopFrame.Security.Models;

namespace HopFrame.Api.Logic;

public interface IAuthLogic {
    Task<LogicResult<SingleValueResult<string>>> Login(UserLogin login);

    Task<LogicResult<SingleValueResult<string>>> Register(UserRegister register);

    Task<LogicResult<SingleValueResult<string>>> Authenticate();

    Task<LogicResult> Logout();

    Task<LogicResult> Delete(UserPasswordValidation validation);
}