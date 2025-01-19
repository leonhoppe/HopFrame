using Microsoft.EntityFrameworkCore;

namespace HopFrame.Tests.Web.Models;

public class MyDbContext : DbContext {
    public DbSet<MyTable> Table1 { get; set; }
    public DbSet<MyTable2> Table2 { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseInMemoryDatabase(nameof(MyDbContext));
    }
}