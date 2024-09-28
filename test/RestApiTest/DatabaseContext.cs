using HopFrame.Database;
using Microsoft.EntityFrameworkCore;
using RestApiTest.Models;

namespace RestApiTest;

public class DatabaseContext : HopDbContextBase {

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Address> Addresses { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite("Data Source=C:\\Users\\Remote\\Documents\\Projekte\\HopFrame\\test\\RestApiTest\\bin\\Debug\\net8.0\\test.db;Mode=ReadWrite;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Address)
            .WithOne(a => a.Employee);
    }
}