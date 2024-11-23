using HopFrame.Api.Models;
using HopFrame.Security.Models;

namespace HopFrame.Api.Logic;

public interface IAuthLogic {
    Task<LogicResult<SingleValueResult<string>>> Login(UserLogin login);

    Task<LogicResult<SingleValueResult<string>>> Register(UserRegister register);

    /// <summary>
    /// Reassures that the user has a valid refresh token and generates a new access token
    /// </summary>
    /// <returns>The newly generated access token</returns>
    Task<LogicResult<SingleValueResult<string>>> Authenticate();

    Task<LogicResult> Logout();

    /// <summary>
    /// Deletes the user account that called the endpoint if the provided password is correct
    /// </summary>
    /// <param name="validation">The password od the user</param>
    /// <returns></returns>
    Task<LogicResult> Delete(UserPasswordValidation validation);
}