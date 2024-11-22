# Auth Service
The `IAuthService` provides some useful methods to handle user authentication (login/register).

## Usage
Simply define the `IAuthService` as a dependency

```csharp
public interface IAuthService {
    Task Register(UserRegister register);
    Task<bool> Login(UserLogin login);
    Task Logout();

    Task<Token> RefreshLogin();
    Task<bool> IsLoggedIn();
}
```
## Automatically refresh user sessions
1. Make sure you have implemented the `AuthMiddleware` how it's described in step 5 of the [installation](./installation.md).

2. After that, the access token of the user gets automatically refreshed as long as the refresh token is valid.
