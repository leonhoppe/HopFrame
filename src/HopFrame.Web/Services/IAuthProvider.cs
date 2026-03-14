#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
namespace HopFrame.Web.Services;

/// The provider used by the HopFrame ui to determine
public interface IAuthProvider {

    /// <summary>
    /// Determines if the current request is performed by an authenticated user
    /// </summary>
    /// <param name="claim">The claim needed to access the request</param>
    /// <returns>True if the request is permitted and False if not</returns>
    public Task<bool> IsAuthenticated(string? claim, CancellationToken cancellationToken);

}