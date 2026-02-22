namespace HopFrame.Core.Configuration;

/**
 * The configuration for the library
 */
public sealed class HopFrameConfig {
    /** The configurations for the table repositories */
    public IList<TableConfig> Tables { get; set; } = new List<TableConfig>();
    
    internal HopFrameConfig() {}
}