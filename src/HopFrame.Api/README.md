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

```csharp
builder.Services.AddDbContext<DatabaseContext>();
//builder.Services.AddHopFrame<DatabaseContext>();
services.AddHopFrameNoEndpoints<TDbContext>();
```

# Services added in this module
You can use these services by specifying them as a dependency. All of them are scoped dependencies.

## LogicResult
Logic result is an extension of the ActionResult for an ApiController. It provides simple Http status results with either a message or data by specifying the generic type.

```csharp
public class LogicResult : ILogicResult {
    public static LogicResult Ok();

    public static LogicResult BadRequest();

    public static LogicResult BadRequest(string message);

    public static LogicResult Forbidden();

    public static LogicResult Forbidden(string message);

    public static LogicResult NotFound();

    public static LogicResult NotFound(string message);

    public static LogicResult Conflict();

    public static LogicResult Conflict(string message);

    public static LogicResult Forward(LogicResult result);

    public static LogicResult Forward<T>(ILogicResult<T> result);

    public static implicit operator ActionResult(LogicResult v);
}

public class LogicResult<T> : ILogicResult<T> {
    public static LogicResult<T> Ok();

    public static LogicResult<T> Ok(T result);
    
    ...
}
```

## IAuthLogic
This service handles all logic needed to provide the authentication endpoints by using the LogicResults.

```csharp
public interface IAuthLogic {
    Task<LogicResult<SingleValueResult<string>>> Login(UserLogin login);

    Task<LogicResult<SingleValueResult<string>>> Register(UserRegister register);

    Task<LogicResult<SingleValueResult<string>>> Authenticate();

    Task<LogicResult> Logout();

    Task<LogicResult> Delete(UserPasswordValidation validation);
}
```