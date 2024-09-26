using HopFrame.Api.Logic;
using HopFrame.Api.Models;
using HopFrame.Security.Authorization;
using HopFrame.Security.Models;
using Microsoft.AspNetCore.Mvc;

namespace HopFrame.Api.Controller;

[ApiController]
[Route("authentication")]
public class SecurityController(IAuthLogic auth) : ControllerBase {

    [HttpPut("login")]
    public async Task<ActionResult<SingleValueResult<string>>> Login([FromBody] UserLogin login) {
        return await auth.Login(login);
    }

    [HttpPost("register")]
    public async Task<ActionResult<SingleValueResult<string>>> Register([FromBody] UserRegister register) {
        return await auth.Register(register);
    }

    [HttpGet("authenticate")]
    public async Task<ActionResult<SingleValueResult<string>>> Authenticate() {
        return await auth.Authenticate();
    }

    [HttpDelete("logout"), Authorized]
    public async Task<ActionResult> Logout() {
        return await auth.Logout();
    }

    [HttpDelete("delete"), Authorized]
    public async Task<ActionResult> Delete([FromBody] UserPasswordValidation validation) {
        return await auth.Delete(validation);
    }
    
}