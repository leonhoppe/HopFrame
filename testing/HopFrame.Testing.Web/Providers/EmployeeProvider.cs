using HopFrame.Web.Admin;
using Microsoft.EntityFrameworkCore;
using HopFrame.Testing.Api.Models;

namespace HopFrame.Testing.Web.Providers;

public class EmployeeProvider(DatabaseContext context) : ModelProvider<Employee> {
    
    public override async Task<IEnumerable<Employee>> ReadAll() {
        return await context.Employees
            .Include(e => e.Address)
            .ToArrayAsync();
    }

    public override async Task<Employee> Create(Employee model) {
        await context.Employees.AddAsync(model);
        await context.SaveChangesAsync();
        return model;
    }

    public override async Task<Employee> Update(Employee model) {
        context.Employees.Update(model);
        await context.SaveChangesAsync();
        return model;
    }

    public override async Task Delete(Employee model) {
        context.Employees.Remove(model);
        await context.SaveChangesAsync();
    }
}