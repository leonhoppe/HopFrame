using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseTest.Controllers;

[ApiController]
[Route("test")]
public class TestController(ITokenContext userContext) : ControllerBase {

    [HttpGet("permissions"), Authorized]
    public ActionResult<IList<string>> Permissions() {
        return new ActionResult<IList<string>>(userContext.User.Permissions);
    }
    
}