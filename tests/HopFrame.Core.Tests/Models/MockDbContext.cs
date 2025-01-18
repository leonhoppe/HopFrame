using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Tests.Models;

// A mock DbContext for testing purposes
public class MockDbContext : DbContext {
    public DbSet<MockModel> Models { get; set; }
    public DbSet<MockModel2> Models2 { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase(nameof(MockDbContext));
    }
}