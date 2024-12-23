using HopFrame.Security.Authentication.OpenID.Models;

namespace HopFrame.Security.Authentication.OpenID;

public interface IOpenIdAccessor {
    public static string DefaultCallback;
    
    Task<OpenIdConfiguration> LoadConfiguration();
    Task<OpenIdToken> RequestToken(string code);
    Task<string> ConstructAuthUri(string state = null);
    Task<OpenIdIntrospection> InspectToken(string token);
    Task<OpenIdToken> RefreshAccessToken(string refreshToken);
}