using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database.Tests.Data;

public class DatabaseContext : HopDbContextBase {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
}