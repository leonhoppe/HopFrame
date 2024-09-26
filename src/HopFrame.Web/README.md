# HopFrame Web module
This module contains useful helpers for Blazor Apps and an Admin Dashboard.

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

# Services added in this module
You can use these services by specifying them as a dependency. All of them are scoped dependencies.

## IAuthService
This service handles all the authentication like login or register. It properly creates all tokens so the user can be identified

```csharp
public interface IAuthService {
    Task Register(UserRegister register);
    Task<bool> Login(UserLogin login);
    Task Logout();

    Task<TokenEntry> RefreshLogin();
    Task<bool> IsLoggedIn();
}
```
