using HopFrame.Web.Admin;
using HopFrame.Web.Admin.Models;
using RestApiTest.Models;

namespace FrontendTest;

public class AdminContext : AdminPagesContext {

    public AdminPage<Address> Addresses { get; set; }
    public AdminPage<Employee> Employees { get; set; }
    
}