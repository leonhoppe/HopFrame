# HopFrame Authentication

With the provided HopFrame services, you can secure your endpoints so that only logged-in users or users with the right permissions can access the endpoint.

## Usage
You can secure your endpoints by adding the `Authorized` attribute.

```csharp
// Everyone can access this endpoint
[HttpGet("hello")]
public ActionResult<string> HelloWorld() {
    return "Hello, World!";
}
```

```csharp
// Only logged-in users can access this endpoint
[HttpGet("hello"), Authorized]
public ActionResult<string> HelloWorld() {
    return "Hello, World!";
}
```

```csharp
// Only logged-in users with the specified permissions can access this endpoint
[HttpGet("hello"), Authorized("test.permission", "test.permission.another")]
public ActionResult<string> HelloWorld() {
    return "Hello, World!";
}
```
