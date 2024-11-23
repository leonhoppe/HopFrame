# HopFrame
A simple backend management api for ASP.NET Core Web APIs

# Features
- [x] Database management
- [x] User authentication
- [x] Permission management
- [x] Generated frontend administration boards

# Usage
There are two different versions of HopFrame, either the Web API version or the full Blazor web version.

## Ho to use the Web API version

> **Hint:** For more information about the HopFrame installation and usage go to the [docs](./docs).

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

## How to use the Blazor API

1. Add the HopFrame.Web library to your project

   ```
   dotnet add package HopFrame.Web
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
   
4. Add the authentication middleware to your app

   ```csharp
   app.UseMiddleware<AuthMiddleware>();
   ```
   
5. Add the HopFrame pages to your Razor components

   ```csharp
   app.MapRazorComponents<App>()
    .AddHopFrameAdminPages()
    .AddInteractiveServerRenderMode();
   ```
