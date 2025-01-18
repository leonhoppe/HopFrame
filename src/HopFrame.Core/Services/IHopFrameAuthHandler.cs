namespace HopFrame.Core.Services;

public interface IHopFrameAuthHandler {
    public Task<bool> IsAuthenticatedAsync(string? policy);
    public Task<string> GetCurrentUserDisplayNameAsync();
}