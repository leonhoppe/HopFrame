# HopFrame Models
All models used by the RestAPI are listed below

## SingleValueResult
```csharp
public struct SingleValueResult<TValue>(TValue value) {
    public TValue Value { get; set; } = value;
}
```

## UserPasswordValidation
```csharp
public sealed class UserPasswordValidation {
    public string Password { get; set; }
}
```
