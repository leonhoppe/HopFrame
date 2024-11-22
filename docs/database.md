# Database initialization
You also need to initialize the data source with the tables from HopFrame.

## Create a DbContext

1. Create a c# class that inherits from the `HopDbContextBase` and add a data source (In the example Sqlite is used)\
   **IMPORTANT:** You need to leave the `base.OnConfiguring(optionsBuilder)` in place so the HopFrame model relations are set correctly.

    ```csharp
    public class DatabaseContext : HopDbContextBase {
       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
           base.OnConfiguring(optionsBuilder);
    
           optionsBuilder.UseSqlite("...");
       }
    }
    ```

2. Register the `DatabaseContext` as a service

   ```csharp
   builder.Services.AddDbContext<DatabaseContext>();
   ```

3. Create a database migration

   ```bash
   dotnet ef migrations add Initial
   ```

4. Apply the migration to the data source

   ```bash
   dotnet ef database update
   ```
