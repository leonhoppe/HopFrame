# Ho to use the Web API version
This Installation adds all HopFrame [endpoints](../endpoints) and [services](../services) to the application.

1. Add the HopFrame.Api library to your project:

   ```
   dotnet add package HopFrame.Api
   ```

2. Create a [DbContext](./Database.md) that inherits the ``HopDbContext`` and add a data source
   <p>&nbsp;</p>

3. Add the HopFrame services to your application, provide the previously created `DatabaseContext` that inherits from `HopDbContextBase`

   ```csharp
   builder.Services.AddHopFrame<DatabaseContext>();
   ```
