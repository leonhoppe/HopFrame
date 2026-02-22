namespace HopFrame.Core.Configuration;

/**
 * The configuration for a table
 */
public class TableConfig {
    /** The unique identifier for the table (usually the name of the model) */
    public required string Identifier { get; init; }
    
    /** The configurations for the properties of the model */
    public IList<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();

    /** The type of the model */
    public required Type TableType { get; set; }

    /** The type identifier for the repository */
    public required Type RepositoryType { get; set; }
    
    /** the url of the table page */
    public required string Route { get; set; }
    
    /** The displayed name of the table */
    public required string DisplayName { get; set; }
    
    /** A short description for the table */
    public string? Description { get; set; }
    
    /** The place (from top to bottom) that the table will appear in on the sidebar */
    public int OrderIndex { get; set; }
    
    internal TableConfig() {}
}