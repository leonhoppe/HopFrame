using HopFrame.Security.Authentication.OpenID;
using Microsoft.AspNetCore.Mvc;
using HopFrame.Security.Authentication.OpenID.Models;

namespace HopFrame.Testing.Api.Controllers;

public class AuthController(IOpenIdAccessor accessor) : Controller {

    [HttpGet("auth/callback")]
    public async Task<ActionResult<string>> Callback([FromQuery] string code, [FromQuery] string state) {
        if (string.IsNullOrEmpty(code)) {
            return BadRequest("Authorization code is missing");
        }

        var token = await accessor.RequestToken(code);
        return Ok(token.AccessToken);
    }

    [HttpGet("auth")]
    public async Task<ActionResult> Authenticate() {
        return Redirect(await accessor.ConstructAuthUri());
    }

    [HttpGet("check")]
    public async Task<ActionResult<OpenIdIntrospection>> Check([FromQuery] string token) {
        return Ok(await accessor.InspectToken(token));
    }
}