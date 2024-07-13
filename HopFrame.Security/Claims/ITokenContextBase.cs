using HopFrame.Database.Models;

namespace HopFrame.Security.Claims;

public interface ITokenContextBase {
    bool IsAuthenticated { get; }
    User User { get; }
    Guid AccessToken { get; }
}