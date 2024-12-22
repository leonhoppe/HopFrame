using HopFrame.Security.Options;

namespace HopFrame.Security.Authentication;

public class HopFrameAuthenticationOptions : OptionsFromConfiguration {
    public override string Position { get; } = "HopFrame:Authentication";
    
    public TimeSpan AccessTokenTime => AccessToken is null ? new(0, 0, 5, 0) : AccessToken.ConstructTimeSpan;
    public TimeSpan RefreshTokenTime  => RefreshToken is null ? new(30, 0, 0, 0) : RefreshToken.ConstructTimeSpan;

    public TokenTime AccessToken { get; set; }
    public TokenTime RefreshToken { get; set; }
    
    public class TokenTime {
        public int Days { get; set; }
        public int Hours { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public TimeSpan ConstructTimeSpan => new(Days, Hours, Minutes, Seconds);
    }
}