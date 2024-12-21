# HopFrame Authentication

HopFrame uses a token system with a short term access token and a long term refresh token for authenticating users.
These tokens are usually provided to the endpoints of the API / Blazor Pages through Cookies:

| Cookie key                     | Cookie value sample                    | Description                 |
|--------------------------------|----------------------------------------|-----------------------------|
| HopFrame.Security.RefreshToken | `42047983-914d-418b-841a-4382614231be` | The long term refresh token |
| HopFrame.Security.AccessToken  | `d39c9432-0831-42df-8844-5e2b70f03eda` | The short term access token |

The advantage of these cookies is that they are automatically set by the backend and delete themselves, when they are
no longer valid.

The access token can also be delivered through a header called `HopFrame.Authentication` or `Token`.
It can also be delivered through a query parameter called `token`. This simplifies requests for images for example
because you can directly specify the url in the img tag in html.

## Authentication configuration

You can also configure the time span that the tokens are valid using the `appsettings.json` or environment variables
by configuring your configuration to load these.
>**Hint**: Configuring your application to use environment variables works by simply adding
> `builder.Configuration.AddEnvironmentVariables();` to your startup configuration before you add the
> custom configurations / HopFrame services.

You can specify `Seconds`, `Minutes`, `Hours` and `Days` for either of the two token types.
These get combined to a single time span.

#### Configuration example
```json
  "HopFrame": {
    "Authentication": {
      "AccessToken": {
        "Minutes": 30
      },
      "RefreshToken": {
        "Days": 10,
        "Hours": 5
      }
    }
  }
```

#### Environment variables example
```dotenv
HOPFRAME__AUTHENTICATION__ACCESSTOKEN__MINUTES=30
HOPFRAME__AUTHENTICATION__REFRESHTOKEN__DAYS=10
HOPFRAME__AUTHENTICATION__REFRESHTOKEN__HOURS=5
```

## API tokens

API tokens are useful to use in automation environments that need to access an endpoint or page of your application.
The HopFrame supports this natively and no further configuration is required in order to use them.

### Create an api token

You can create an api token via the `ITokenRepository`:
```csharp
tokens.CreateApiToken(user, DateTime.MaxValue);
```

This creates a new api token that is valid until the provided DateTime has passed. Note that in the database and the token
model the `CreatedAt` property represents the expiration date on an api token. For security reasons the api token by default
has no permissions. This allows you to create tokens that are just permitted to perform a single action. Note that a token
associated to a user can also have more permissions than the user itself so make sure to properly secure the creation process.

### Add permissions to an api token

You can add permissions to an api token like you would to a normal user or group:

```csharp
permissions.AddPermission(apiToken, "token.permission");
```
