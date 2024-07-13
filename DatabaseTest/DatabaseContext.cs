using HopFrame.Database;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTest;

public class DatabaseContext : HopDbContextBase {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite("Data Source=C:\\Users\\Remote\\Documents\\Projekte\\HopFrame\\DatabaseTest\\bin\\Debug\\net7.0\\test.db;Mode=ReadWrite;");
    }
}