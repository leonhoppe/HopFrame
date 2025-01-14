using HopFrame.Core.Services;

namespace HopFrame.Testing.Services;

public class AuthService : IHopFrameAuthHandler {
    public Task<bool> IsAuthenticatedAsync(string? policy) {
        return Task.FromResult(true);
    }
    public Task<string> GetCurrentUserDisplayNameAsync() {
        return Task.FromResult("Leon Hoppe");
    }
}