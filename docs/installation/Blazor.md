## How to use the Blazor API
This Installation adds all HopFrame [pages](../pages) and [services](../services) to the application.

1. Add the HopFrame.Web library to your project

   ```
   dotnet add package HopFrame.Web
   ```

2. Create a [DbContext](./Database.md) that inherits the ``HopDbContext`` and add a data source
   <p>&nbsp;</p>

3. Add the HopFrame services to your application, provide the previously created `DatabaseContext` that inherits from `HopDbContextBase`

   ```csharp
   builder.Services.AddHopFrame<DatabaseContext>();
   ```
   
4. **Optional:** You can also add your [AdminContext](../admin)

   ```csharp
   builder.Services.AddAdminContext<AdminContext>();
   ```

5. Add the authentication middleware to your app

   ```csharp
   app.UseMiddleware<AuthMiddleware>();
   ```

6. Add the HopFrame pages to your Razor components

   ```csharp
   app.MapRazorComponents<App>()
    .AddHopFrameAdminPages()
    .AddInteractiveServerRenderMode();
   ```
