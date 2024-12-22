using System.Text.Json.Serialization;

namespace HopFrame.Security.Authentication.OpenID.Models;

public sealed class OpenIdIntrospection {
    [JsonPropertyName("iss")]
    public string Issuer { get; set; }

    [JsonPropertyName("sub")]
    public string Subject { get; set; }

    [JsonPropertyName("aud")]
    public string Audience { get; set; }

    [JsonPropertyName("exp")]
    public long Expiration { get; set; }

    [JsonPropertyName("iat")]
    public long IssuedAt { get; set; }

    [JsonPropertyName("auth_time")]
    public long AuthTime { get; set; }

    [JsonPropertyName("acr")]
    public string Acr { get; set; }

    [JsonPropertyName("amr")]
    public List<string> AuthenticationMethods { get; set; }

    [JsonPropertyName("sid")]
    public string SessionId { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; }

    [JsonPropertyName("preferred_username")]
    public string PreferredUsername { get; set; }

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; }

    [JsonPropertyName("groups")]
    public List<string> Groups { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    [JsonPropertyName("client_id")]
    public string ClientId { get; set; }
}