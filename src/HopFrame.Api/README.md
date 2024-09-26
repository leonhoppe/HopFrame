# HopFrame API module
This module contains some useful endpoints for user login / register management.

## Ho to use the Web API version

1. Add the HopFrame.Api library to your project:

   ```
   dotnet add package HopFrame.Api
   ```

2. Create a DbContext that inherits the ``HopDbContext`` and add a data source

   ```csharp
   public class DatabaseContext : HopDbContextBase {
       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
           base.OnConfiguring(optionsBuilder);

           optionsBuilder.UseSqlite("...");
       }
   }
   ```

3. Add the DbContext and HopFrame to your services

   ```csharp
   builder.Services.AddDbContext<DatabaseContext>();
   builder.Services.AddHopFrame<DatabaseContext>();
   ```

# Endpoints
By default, the module provides a controller for handling authentication based requests by the user.
You can explore the contoller by the build in swagger site from ASP .NET.

## Disable the Endpoints
If you don't want to include these endpoints you need to comment out the AddHopFrame line and only add the Auth middleware:
```csharp
builder.Services.AddDbContext<DatabaseContext>();
//builder.Services.AddHopFrame<DatabaseContext>();
services.AddHopFrameAuthentication<TDbContext>();
```
