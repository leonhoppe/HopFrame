# HopFrame base models
All models listed below are part of the core HopFrame components and accessible in all installation variations

> **Note:** All properties of the models that are `virtual` are relational properties and don't directly correspond to columns in the database.

## User
```csharp
public class User : IPermissionOwner {
    public Guid Id { get; init; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual List<Permission> Permissions { get; set; }
    public virtual List<Token> Tokens { get; set; }
}
```

## PermissionGroup
```csharp
public class PermissionGroup : IPermissionOwner {
    public string Name { get; init; }
    public bool IsDefaultGroup { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual List<Permission> Permissions { get; set; }
}
```

## Permission
```csharp
public class Permission {
    public long Id { get; init; }
    public string PermissionName { get; set; }
    public DateTime GrantedAt { get; set; }
    public virtual User User { get; set; }
    public virtual PermissionGroup Group { get; set; }
    public virtual Token Token { get; set; }
}
```

## Token
```csharp
public class Token : IPermissionOwner {
    public int Type { get; set; }
    public Guid Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual User Owner { get; set; }
    public virtual List<Permission> Permissions { get; set; }
}
```

## UserLogin
```csharp
public class UserLogin {
    public string Email { get; set; }
    public string Password { get; set; }
}
```

## UserRegister
```csharp
public class UserRegister {
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
```

## UserCreator
```csharp
public class UserCreator {
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public virtual List<string> Permissions { get; set; }
}
```

## IPermissionOwner
```csharp
public interface IPermissionOwner;
```
