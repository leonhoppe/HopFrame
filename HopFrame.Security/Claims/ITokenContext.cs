using HopFrame.Database.Models;

namespace HopFrame.Security.Claims;

public interface ITokenContext {
    
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
    Guid AccessToken { get; }
}