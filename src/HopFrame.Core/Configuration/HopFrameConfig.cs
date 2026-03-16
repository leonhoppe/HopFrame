using System.Security.Claims;

namespace HopFrame.Core.Configuration;

/// <summary>
/// The configuration for the library
/// </summary>
public sealed class HopFrameConfig {
    /// The configurations for the table repositories
    public IList<TableConfig> Tables { get; init; } = new List<TableConfig>();

    /// The general claim a user needs to access the ui
    public string? BaseClaim { get; set; }

    /// Determines the type of claim used to check for access
    public string ClaimType { get; set; } = ClaimTypes.Role;

    /// Determines the url a user gets redirected to if he isn't authorized
    public string RedirectUnauthorized { get; set; } = "/";

    /// Determines if an authorization check needs to be performed
    public bool AllowAnonymousAccess { get; set; } = true;

    /// Determines the title of the admin ui
    public string CompanyName { get; set; } = "HopFrame";
    
    internal HopFrameConfig() {}
}
