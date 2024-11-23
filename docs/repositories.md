# HopFrame Repositories
The HopFrame provies repositories for the various build in database models as an abstraction around the `HopDbContext` to ensure, that the data is proccessed and saved correctly.

## Overview
The repositories can also be used by simply defining them as a dependency in your service / controller.

### User Repository

```csharp
public interface IUserRepository {
    Task<IList<User>> GetUsers();

    Task<User> GetUser(Guid userId);

    Task<User> GetUserByEmail(string email);

    Task<User> GetUserByUsername(string username);
    
    Task<User> AddUser(User user);
    
    Task UpdateUser(User user);

    Task DeleteUser(User user);

    Task<bool> CheckUserPassword(User user, string password);

    Task ChangePassword(User user, string password);
}
```

### Group Repository

```csharp
public interface IGroupRepository {
    Task<IList<PermissionGroup>> GetPermissionGroups();

    Task<IList<PermissionGroup>> GetDefaultGroups();

    Task<IList<PermissionGroup>> GetUserGroups(User user);

    Task<PermissionGroup> GetPermissionGroup(string name);

    Task EditPermissionGroup(PermissionGroup group);

    Task<PermissionGroup> CreatePermissionGroup(PermissionGroup group);

    Task DeletePermissionGroup(PermissionGroup group);
}
```

### Permission Repository

```csharp
public interface IPermissionRepository {
    Task<bool> HasPermission(IPermissionOwner owner, params string[] permissions);
    
    Task<Permission> AddPermission(IPermissionOwner owner, string permission);
    
    Task RemovePermission(IPermissionOwner owner, string permission);

    Task<IList<string>> GetFullPermissions(IPermissionOwner owner);
}
```

### Token Repository

```csharp
public interface ITokenRepository {
    Task<Token> GetToken(string content);

    Task<Token> CreateToken(int type, User owner);

    Task DeleteUserTokens(User owner);
}
```
