# HopFrame Models
All models used by the RestAPI are listed below

## SingleValueResult
```csharp
public struct SingleValueResult<TValue>(TValue value) {
    public TValue Value { get; set; } = value;
}
```

## UserPasswordValidation
```csharp
public sealed class UserPasswordValidation {
    public string Password { get; set; }
}
```

## OpenIdConfiguration
```csharp
public sealed class OpenIdConfiguration {
    public string Issuer { get; set; }
    public string AuthorizationEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string UserinfoEndpoint { get; set; }
    public string EndSessionEndpoint { get; set; }
    public string IntrospectionEndpoint { get; set; }
    public string RevocationEndpoint { get; set; }
    public string DeviceAuthorizationEndpoint { get; set; }
    public List<string> ResponseTypesSupported { get; set; }
    public List<string> ResponseModesSupported { get; set; }
    public string JwksUri { get; set; }
    public List<string> GrantTypesSupported { get; set; }
    public List<string> IdTokenSigningAlgValuesSupported { get; set; }
    public List<string> SubjectTypesSupported { get; set; }
    public List<string> TokenEndpointAuthMethodsSupported { get; set; }
    public List<string> AcrValuesSupported { get; set; }
    public List<string> ScopesSupported { get; set; }
    public bool RequestParameterSupported { get; set; }
    public List<string> ClaimsSupported { get; set; }
    public bool ClaimsParameterSupported { get; set; }
    public List<string> CodeChallengeMethodsSupported { get; set; }
}
```

## OpenIdIntrospection
```csharp
public sealed class OpenIdIntrospection {
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public string Audience { get; set; }
    public long Expiration { get; set; }
    public long IssuedAt { get; set; }
    public long AuthTime { get; set; }
    public string Acr { get; set; }
    public List<string> AuthenticationMethods { get; set; }
    public string SessionId { get; set; }
    public string Email { get; set; }
    public bool EmailVerified { get; set; }
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string PreferredUsername { get; set; }
    public string Nickname { get; set; }
    public List<string> Groups { get; set; }
    public bool Active { get; set; }
    public string Scope { get; set; }
    public string ClientId { get; set; }
}
```

## OpenIdToken
```csharp
public sealed class OpenIdToken {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string IdToken { get; set; }
}
```
