# HopFrame Security module
this module contains all handlers for the login and register validation. It also checks the user permissions.

# Services added in this module
You can use these services by specifying them as a dependency. All of them are scoped dependencies.

## ITokenContext
This service provides the information given by the current request

```csharp
public interface ITokenContext {
    bool IsAuthenticated { get; }
    
    User User { get; }
    
    Guid AccessToken { get; }
}
```

## IUserService
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

## IPermissionService
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
