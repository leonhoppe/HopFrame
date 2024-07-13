using HopFrame.Api.Controller;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseTest.Controllers;

[ApiController]
public class TestController(DatabaseContext context) : SecurityController<DatabaseContext>(context) {
    
}