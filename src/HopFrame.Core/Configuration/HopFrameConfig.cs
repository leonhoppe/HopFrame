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

    /// Determines if the search bar at the top of every page should be visible
    public bool ShowSearchBar { get; set; } = true;

    /// The categories will be displayed as they are entered in this list. If empty, the categories will be sorted alphabetically
    public string?[]? CategoryOrder { get; set; }

    /// The icons that will be displayed next to the categories
    public Dictionary<string, string> CategoryIcons { get; set; } = new();
}
