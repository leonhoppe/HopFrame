# Authentication

The HopFrame is a powerful tool to manage your backend data. So you probably don't want anybody to access the pages.
Luckily the HopFrame supports policy based authentication. By default, everybody is allowed to access the whole
HopFrame, but you can restrict that by registering a scoped service implementing the `IHopFrameAuthHandler`.
If no service is registered, the default handler gets registered, but it lets any traffic pass.

## Example

Create a service that handles authentication:

```C#
public class AuthService(IAuthStore store) : IHopFrameAuthHandler {

    public async Task<bool> IsAuthenticatedAsync(string? policy) {
        var currentUser = await store.GetCurrentUser();
        return await store.IsPermitted(currentUser, policy);
    }
    
    public async Task<string> GetCurrentUserDisplayNameAsync() {
        var currentUser = await store.GetCurrentUser();
        return currentUser.FullName;
    }
    
}
```

Now register it in the DI container:

```C#
builder.Services.AddScoped<IHopFrameAuthHandler, AuthService>();
```

**Hint:** You can display the current users name in the ui by enabling the feature in
the [HopFrameConfig](HopFrameConfig.md#displayuserinfo).
