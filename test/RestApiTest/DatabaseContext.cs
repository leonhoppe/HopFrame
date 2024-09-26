using HopFrame.Database;
using Microsoft.EntityFrameworkCore;

namespace RestApiTest;

public class DatabaseContext : HopDbContextBase {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite("Data Source=C:\\Users\\Remote\\Documents\\Projekte\\HopFrame\\test\\RestApiTest\\bin\\Debug\\net8.0\\test.db;Mode=ReadWrite;");
    }
}