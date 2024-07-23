using HopFrame.Database.Models;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace RestApiTest.Controllers;

[ApiController]
[Route("test")]
public class TestController(ITokenContext userContext) : ControllerBase {

    [HttpGet("permissions"), Authorized]
    public ActionResult<IList<Permission>> Permissions() {
        return new ActionResult<IList<Permission>>(userContext.User.Permissions);
    }
    
}