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

    public HopFrameAuthenticationOptions.TokenTime RefreshToken { get; set; } = new() {
        Days = 30
    };

    public CachingOptions Cache { get; set; } = new() {
        Enabled = true,
        Configuration = new() {
            Enabled = true,
            TTL = new() {
                Minutes = 10
            }
        },
        Auth = new() {
            Enabled = true,
            TTL = new() {
                Seconds = 30
            }
        },
        Inspection = new() {
            Enabled = true,
            TTL = new() {
                Minutes = 2
            }
        }
    };
    
    public class CachingTypeOptions {
        public bool Enabled { get; set; }
        public HopFrameAuthenticationOptions.TokenTime TTL { get; set; }
    }
    
    public class CachingOptions {
        public bool Enabled { get; set; }
        public CachingTypeOptions Configuration { get; set; }
        public CachingTypeOptions Auth { get; set; }
        public CachingTypeOptions Inspection { get; set; }
    }
}