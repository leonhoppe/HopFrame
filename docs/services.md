# HopFrame Services
This page describes all services provided by the HopFrame.
You can use these services by specifying them as a dependency. All of them are scoped dependencies.

## HopFrame.Security
### ITokenContext
This service provides the information given by the current request

```csharp
public interface ITokenContext {
    bool IsAuthenticated { get; }
    
    User User { get; }
    
    Guid AccessToken { get; }
}
```

### IUserService
This service simplifies the data access of the user table in the database.

```csharp
public interface IUserService {
    Task<IList<User>> GetUsers();

    Task<User> GetUser(Guid userId);

    Task<User> GetUserByEmail(string email);

    Task<User> GetUserByUsername(string username);
    
    Task<User> AddUser(UserRegister user);

    Task UpdateUser(User user);

    Task DeleteUser(User user);

    Task<bool> CheckUserPassword(User user, string password);

    Task ChangePassword(User user, string password);
}
```

### IPermissionService
This service handles all permission and group interactions with the data source.

```csharp
public interface IPermissionService {
    Task<bool> HasPermission(string permission, Guid user);

    Task<IList<PermissionGroup>> GetPermissionGroups();

    Task<PermissionGroup> GetPermissionGroup(string name);

    Task EditPermissionGroup(PermissionGroup group);

    Task<IList<PermissionGroup>> GetUserPermissionGroups(User user);

    Task RemoveGroupFromUser(User user, PermissionGroup group);

    Task<PermissionGroup> CreatePermissionGroup(string name, bool isDefault = false, string description = null);

    Task DeletePermissionGroup(PermissionGroup group);

    Task<Permission> GetPermission(string name, IPermissionOwner owner);

    Task AddPermission(IPermissionOwner owner, string permission);

    Task RemovePermission(Permission permission);

    Task<string[]> GetFullPermissions(string user);
}
```

## HopFrame.Api
### LogicResult
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

### IAuthLogic
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

## HopFrame.Web
### IAuthService
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
