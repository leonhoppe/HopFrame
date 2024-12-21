using HopFrame.Security.Options;

namespace HopFrame.Security.Authentication.OpenID.Options;

public sealed class OpenIdOptions : OptionsFromConfiguration {
    public override string Position { get; } = "HopFrame:Authentication:OpenID";

    public bool Enabled { get; set; } = false;
    public bool GenerateUsers { get; set; } = true;

    public string Issuer { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Callback { get; set; }
}