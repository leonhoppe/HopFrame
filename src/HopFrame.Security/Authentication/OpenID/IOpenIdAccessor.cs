using HopFrame.Security.Authentication.OpenID.Models;

namespace HopFrame.Security.Authentication.OpenID;

public interface IOpenIdAccessor {
    Task<OpenIdConfiguration> LoadConfiguration();
    Task<OpenIdToken> RequestToken(string code);
    Task<string> ConstructAuthUri(string state = null);
    Task<OpenIdIntrospection> InspectToken(string token);
}