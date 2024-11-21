# HopFrame Authentication

With the provided HopFrame services, you can secure your endpoints and blazor pages so that only logged-in users or users with the right permissions can access the endpoint/page.

## Usage
### Secure your endpoints
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

### Secure your Blazor pages
You can secure your Blazor pages by using the `AuthorizedView` component.
Everything placed inside this component will only be displayed if the authorization was successful.
You can also redirect the user if the authorization fails by specifying a `RedirectIfUnauthorized` url.

```html
<!-- You can either specify one 'Permission', multiple 'Permissions' or none if the user only needs to be logged-in -->
<AuthorizedView Permission="test.permission">
    <p>This paragraph is only visible if the user is logged-in and has the required permission</p>
</AuthorizedView>
```

```html
<!-- This component will redirect the user to the login page if the user is unauthorized -->
<AuthorizedView RedirectIfUnauthorized="/login" />
```
