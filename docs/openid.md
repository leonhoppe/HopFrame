# OpenID Authentication
The HopFrame allows you to use an OpenID provider as your authentication provider for single sign on or better security
etc. To use it, you just simply need to configure it through the `appsettings.json` or environment variables.

>**Note**: The Blazor module has not yet implemented endpoints for the login process, but the middleware is correctly
> configured and the `IOpenIdAccessor` service is also provided for you to easily implement the endpoints yourself.

When you have enabled the integration, new endpoints will also be provided to perform the authentication.
simply use the swagger explorer to look up how the endpoints function. They're all under the subroute
`/api/v1/openid/`.

## Configure the HopFrame to use OpenID authentication

1. Create / Configure your OpenID provider:
    
    - Save the ClientID and Client Secret from the provider, because you need it later.
    - The default redirect uri looks something like this: `https://example.com/api/v1/openid/callback`.
      - **Replace** the origin with the FQDN of your service.
    - In order for the HopFrame to automatically renew expired access tokens you need to enable the `offline_access` scope.
      - The integration also works without doing that, but then you need to reauthenticate every time your access token expires.

2. Configure the HopFrame integration:

   >**Hint**: All of these configuration options can also be defined as environment variables. Use '__'
   > to separate the namespaces like so: `HOPFRAME__AUTHENTICATION__OPENID__ENABLED=true`

   - Add the following lines to your `appsettings.json`:
   ```json
      "HopFrame": {
          "Authentication": {
            "OpenID": {
              "Enabled": true,
              "Issuer": "your-issuer",
              "ClientId": "your-client-id",
              "ClientSecret": "your-client-secret"
            }
          }
      }
   ```
   
   >**Hint**: If you are using Authentik, the issuer url looks something like this: `https://auth.example.com/application/o/application-name/`.
   > Just replace the FQDN and application-name with your configured application.

   - **Optional**: You can also disable the default authentication via the config:

   ```json
      "HopFrame": {
          "Authentication": {
            "DefaultAuthentication": false
          }
      }
   ```
   
   - **Optional**: By default, the HopFrame will cache the api responses to reduce api latency. This can also be configured in the config (the cache can also be completely disabled here):

   ```json
      "HopFrame": {
         "Authentication": {
            "OpenID": {
               "Cache": {
                  "Enabled": true,
                  "Configuration": {
                     "Hours": 5
                  },
                  "Auth": {
                     "Seconds": 90
                  },
                  "Inspection": {
                     "Minutes": 5
                  }
               }
            }
         }
      }
   ```

   - **Optional**: You can also define your own callback endpoint like so (you also need to add / replace the endpoint in the provider settings):

   ```json
      "HopFrame": {
          "Authentication": {
            "OpenID": {
              "Callback": "https://example.com/auth/callback"
            }
          }
      }
   ```
   
   - **Optional**: You can also prevent new users from being created by disabling it in the config:

   ```json
      "HopFrame": {
          "Authentication": {
            "OpenID": {
              "GenerateUsers": false
            }
          }
      }
   ```

## Use the abstraction to integrate OpenID yourself

The HopFrame has a service, that simplifies the communication with the OpenID provider called `IOpenIdAccessor`.
You can inject it like every other service in your application.

```csharp
public interface IOpenIdAccessor {
    
    Task<OpenIdConfiguration> LoadConfiguration();
    
    Task<OpenIdToken> RequestToken(string code, string defaultCallback);
    
    Task<string> ConstructAuthUri(string defaultCallback, string state = null);
    
    Task<OpenIdIntrospection> InspectToken(string token);
    
    Task<OpenIdToken> RefreshAccessToken(string refreshToken);
    
}
```
