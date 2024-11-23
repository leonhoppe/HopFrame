using FrontendTest.Providers;
using HopFrame.Web.Admin;
using HopFrame.Web.Admin.Generators;
using HopFrame.Web.Admin.Models;
using RestApiTest.Models;

namespace FrontendTest;

public class AdminContext : AdminPagesContext {

    public AdminPage<Address> Addresses { get; set; }
    public AdminPage<Employee> Employees { get; set; }

    public override void OnModelCreating(IAdminContextGenerator generator) {
        base.OnModelCreating(generator);

        generator.Page<Employee>()
            .Property(e => e.Address)
            .IsSelector();

        generator.Page<Address>()
            .Property(a => a.Employee)
            .Ignore();

        generator.Page<Address>()
            .Property(a => a.AddressId)
            .IsSelector<Employee>()
            .Parser<Employee>((model, e) => model.AddressId = e.EmployeeId);

        generator.Page<Employee>()
            .ConfigureProvider<EmployeeProvider>()
            .ListingProperty(e => e.Name);

        generator.Page<Address>()
            .ConfigureProvider<AddressProvider>()
            .ListingProperty(a => a.City);
    }
}