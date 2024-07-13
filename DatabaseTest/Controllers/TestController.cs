using HopFrame.Api.Controller;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseTest.Controllers;

[ApiController]
public class TestController(DatabaseContext context, ITokenContext userContext) : SecurityController<DatabaseContext>(context) {

    [HttpGet("permissions"), Authorized]
    public ActionResult<IList<string>> Permissions() {
        return new ActionResult<IList<string>>(userContext.User.Permissions);
    }
    
}