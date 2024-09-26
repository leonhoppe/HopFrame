using HopFrame.Api.Logic;
using HopFrame.Database.Models;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApiTest.Models;

namespace RestApiTest.Controllers;

[ApiController]
[Route("test")]
public class TestController(ITokenContext userContext, DatabaseContext context) : ControllerBase {

    [HttpGet("permissions"), Authorized]
    public ActionResult<IList<Permission>> Permissions() {
        return new ActionResult<IList<Permission>>(userContext.User.Permissions);
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
    
}