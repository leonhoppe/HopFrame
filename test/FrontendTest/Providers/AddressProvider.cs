using HopFrame.Web.Admin;
using Microsoft.EntityFrameworkCore;
using RestApiTest.Models;

namespace FrontendTest.Providers;

public class AddressProvider(DatabaseContext context) : ModelProvider<Address> {
    
    public override async Task<IEnumerable<Address>> ReadAll() {
        return await context.Addresses.ToArrayAsync();
    }

    public override async Task<Address> Create(Address model) {
        await context.Addresses.AddAsync(model);
        await context.SaveChangesAsync();
        return model;
    }

    public override async Task<Address> Update(Address model) {
        context.Addresses.Update(model);
        await context.SaveChangesAsync();
        return model;
    }

    public override async Task Delete(Address model) {
        context.Addresses.Remove(model);
        await context.SaveChangesAsync();
    }
}