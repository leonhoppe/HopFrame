using System.Text.Json.Serialization;

namespace HopFrame.Security.Authentication.OpenID.Models;

public sealed class OpenIdToken {
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }
}