using HopFrame.Database;
using Microsoft.EntityFrameworkCore;

namespace DatabaseTest;

public class DatabaseContext : HopDbContextBase {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlite("Data Source=C:\\Users\\Remote\\Documents\\Projekte\\HopFrame\\DatabaseTest\\test.db;Mode=ReadWrite;");
    }
}