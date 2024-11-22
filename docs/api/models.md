# HopFrame Models
All models used by the RestAPI are listed below

## SingleValueResult
```csharp
public struct SingleValueResult<TValue>(TValue value) {
    public TValue Value { get; set; } = value;
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

## UserPasswordValidation
```csharp
public sealed class UserPasswordValidation {
    public string Password { get; set; }
}
```
