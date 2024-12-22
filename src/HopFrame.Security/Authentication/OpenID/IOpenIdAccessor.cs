using HopFrame.Security.Authentication.OpenID.Models;

namespace HopFrame.Security.Authentication.OpenID;

public interface IOpenIdAccessor {
    Task<OpenIdConfiguration> LoadConfiguration();
    Task<OpenIdToken> RequestToken(string code, string defaultCallback);
    Task<string> ConstructAuthUri(string defaultCallback, string state = null);
    Task<OpenIdIntrospection> InspectToken(string token);
    Task<OpenIdToken> RefreshAccessToken(string refreshToken);
}