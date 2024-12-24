using HopFrame.Api.Models;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace HopFrame.Api.Controller;

[ApiController, Route("api/v1/openid")]
public class OpenIdController(IOpenIdAccessor accessor) : ControllerBase {
    public const string DefaultCallback = "api/v1/openid/callback";

    [HttpGet("redirect")]
    public async Task<IActionResult> RedirectToProvider([FromQuery] string redirectAfter, [FromQuery] int performRedirect = 1) {
        var uri = await accessor.ConstructAuthUri(redirectAfter);

        if (performRedirect == 1) {
            return Redirect(uri);
        }

        return Ok(new SingleValueResult<string>(uri));
    }

    [HttpGet("callback")]
    public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state) {
        if (string.IsNullOrEmpty(code)) {
            return BadRequest("Authorization code is missing");
        }

        var token = await accessor.RequestToken(code);

        if (token is null) {
            return Forbid("Authorization code is not valid");
        }
        
        accessor.SetAuthenticationCookies(token);

        if (string.IsNullOrEmpty(state)) {
            return Ok(new SingleValueResult<string>(token.AccessToken));
        }

        return Redirect(state.Replace("{token}", token.AccessToken));
    }

    [HttpGet("refresh")]
    public async Task<IActionResult> Refresh() {
        var refreshToken = Request.Cookies[ITokenContext.RefreshTokenType];

        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest("Refresh token not provided");

        var token = await accessor.RefreshAccessToken(refreshToken);

        if (token is null)
            return Conflict("Refresh token not valid");
        
        accessor.SetAuthenticationCookies(token);
        
        return Ok(new SingleValueResult<string>(token.AccessToken));
    }

    [HttpDelete("logout")]
    public IActionResult Logout() {
        accessor.Logout();
        return Ok();
    }
    
}