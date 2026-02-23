namespace HopFrame.Core.Configuration;

/// <summary>
/// The configuration for the library
/// </summary>
public sealed class HopFrameConfig {
    /// The configurations for the table repositories
    public IList<TableConfig> Tables { get; set; } = new List<TableConfig>();
    
    internal HopFrameConfig() {}
}