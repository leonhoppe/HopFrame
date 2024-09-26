using HopFrame.Database.Models;

namespace HopFrame.Security.Claims;

public interface ITokenContext {
    
    public const string RefreshTokenType = "HopFrame.Security.RefreshToken";
    public const string AccessTokenType = "HopFrame.Security.AccessToken";
    
    /// <summary>
    /// This field specifies that a valid user is accessing the endpoint
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// The user that is accessing the endpoint
    /// </summary>
    User User { get; }
    
    /// <summary>
    /// The access token the user provided
    /// </summary>
    Token AccessToken { get; }
}