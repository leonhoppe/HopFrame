namespace HopFrame.Core.Services.Implementations;

internal sealed class DefaultAuthHandler : IHopFrameAuthHandler {
    public Task<bool> IsAuthenticatedAsync(string? policy) {
        return Task.FromResult(true);
    }
    public Task<string> GetCurrentUserDisplayNameAsync() {
        return Task.FromResult(string.Empty);
    }
}