using HopFrame.Api.Logic;
using HopFrame.Api.Models;
using HopFrame.Database.Models;
using HopFrame.Database.Repositories;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using HopFrame.Testing.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Testing.Api.Controllers;

[ApiController]
[Route("test")]
public class TestController(ITokenContext userContext, DatabaseContext context, ITokenRepository tokens, IPermissionRepository permissions) : ControllerBase {

    [HttpGet("permissions"), Authorized]
    public async Task<ActionResult<IList<string>>> Permissions() {
        return new ActionResult<IList<string>>(await permissions.GetFullPermissions(userContext.AccessToken));
    }

    [HttpGet("generate")]
    public async Task<ActionResult> GenerateData() {
        var employee = new Employee() {
            Name = "Max Mustermann"
        };

        await context.AddAsync(employee);
        await context.SaveChangesAsync();

        var address = new Address() {
            City = "Musterstadt",
            Country = "Musterland",
            State = "Musterbundesland",
            ZipCode = 12345,
            AddressDetails = "Musterstraße 5",
            Employee = employee
        };

        await context.AddAsync(address);
        await context.SaveChangesAsync();

        return LogicResult.Ok();
    }

    [HttpGet("employees")]
    public async Task<ActionResult<IList<Employee>>> GetEmployees() {
        return LogicResult<IList<Employee>>.Ok(await context.Employees.Include(e => e.Address).ToListAsync());
    }
    
    [HttpGet("addresses")]
    public async Task<ActionResult<IList<Address>>> GetAddresses() {
        return LogicResult<IList<Address>>.Ok(await context.Addresses.Include(e => e.Employee).ToListAsync());
    }

    [HttpGet("token"), Authorized]
    public async Task<ActionResult<SingleValueResult<string>>> GetApiToken() {
        var token = await tokens.CreateApiToken(userContext.User, DateTime.MaxValue);
        await permissions.AddPermission(token, "hopframe.admin");
        await permissions.AddPermission(token, "hopframe.admin.users.read");
        return LogicResult<SingleValueResult<string>>.Ok(token.TokenId.ToString());
    }

    [HttpDelete("token/{tokenId}")]
    public async Task DeleteToken(string tokenId) {
        var token = await tokens.GetToken(tokenId);
        await tokens.DeleteToken(token);
    }

    [HttpGet("url")]
    public ActionResult<string> GetUrl() {
        return Ok(IOpenIdAccessor.DefaultCallback ?? "Not set");
    }
    
}