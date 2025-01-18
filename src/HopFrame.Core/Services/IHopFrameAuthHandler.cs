namespace HopFrame.Core.Services;

/// <summary>
/// This handler is used by the HopFrame for authenticating the user that wants to access the admin ui.
/// By default, everyone is allowed to access the admin ui, but you can implement and register this interface
/// yourself as a scoped dependency if you only want some users to access the admin ui (recommended)
/// </summary>
public interface IHopFrameAuthHandler {
    
    /// <summary>
    /// Gets called by the admin ui in order to verify if the user has all necessary policies to perform an action
    /// </summary>
    /// <param name="policy">If specified, the policy the user needs to perform the action</param>
    /// <returns>True: The user is permitted to perform the action<br /> False: The user is not permitted to perform the action</returns>
    public Task<bool> IsAuthenticatedAsync(string? policy);
    
    /// <summary>
    /// If enabled, this method is used to get the name of the currently logged-in user in order to display it in the admin ui
    /// </summary>
    /// <returns>The display name of the currently logged-in user</returns>
    /// <seealso cref="HopFrame.Core.Config.HopFrameConfigurator.DisplayUserInfo"/>
    public Task<string> GetCurrentUserDisplayNameAsync();
    
}