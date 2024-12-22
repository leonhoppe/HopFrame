using HopFrame.Database;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Tests.Database.Data;

public class DatabaseContext : HopDbContextBase {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
    }
}