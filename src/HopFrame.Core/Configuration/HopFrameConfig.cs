namespace HopFrame.Core.Configuration;

/// <summary>
/// The configuration for the library
/// </summary>
public sealed class HopFrameConfig {
    /// The configurations for the table repositories
    public IList<TableConfig> Tables { get; init; } = new List<TableConfig>();

    /// The custom pages to display in the ui
    public IList<CustomPage> CustomPages { get; init; } = new List<CustomPage>();
    
    internal HopFrameConfig() {}
}

/// <summary>
/// A custom entry in the sidebar and on the dashboard
/// </summary>
/// <param name="Name">The name of the entry</param>
/// <param name="Description">The description of the entry</param>
/// <param name="Icon">The icon of the entry</param>
/// <param name="Route">The href of the entry</param>
/// <param name="OrderIndex">The sort index of the entry</param>
public readonly record struct CustomPage(string Name, string? Description, string Icon, string Route, int OrderIndex);
