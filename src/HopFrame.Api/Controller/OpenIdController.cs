using HopFrame.Api.Models;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authentication.OpenID.Options;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HopFrame.Api.Controller;

[ApiController, Route("api/v1/openid")]
public class OpenIdController(IOpenIdAccessor accessor, IOptions<OpenIdOptions> options) : ControllerBase {
    public const string DefaultCallback = "api/v1/openid/callback";

    [HttpGet("redirect")]
    public async Task<IActionResult> RedirectToProvider([FromQuery] string redirectAfter, [FromQuery] int performRedirect = 1) {
        var uri = await accessor.ConstructAuthUri(DefaultCallback, redirectAfter);

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

        var token = await accessor.RequestToken(code, DefaultCallback);

        if (token is null) {
            return Forbid("Authorization code is not valid");
        }
        
        Response.Cookies.Append(ITokenContext.AccessTokenType, token.AccessToken, new CookieOptions {
            MaxAge = TimeSpan.FromSeconds(token.ExpiresIn),
            HttpOnly = false,
            Secure = true
        });
        Response.Cookies.Append(ITokenContext.RefreshTokenType, token.RefreshToken, new CookieOptions {
            MaxAge = options.Value.RefreshToken.ConstructTimeSpan,
            HttpOnly = false,
            Secure = true
        });

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
            return NotFound("Refresh token not valid");
        
        Response.Cookies.Append(ITokenContext.AccessTokenType, token.AccessToken, new CookieOptions {
            MaxAge = TimeSpan.FromSeconds(token.ExpiresIn),
            HttpOnly = false,
            Secure = true
        });
        
        return Ok(new SingleValueResult<string>(token.AccessToken));
    }

    [HttpDelete("logout")]
    public IActionResult Logout() {
        Response.Cookies.Delete(ITokenContext.RefreshTokenType);
        Response.Cookies.Delete(ITokenContext.AccessTokenType);
        return Ok();
    }
    
}