namespace HopFrame.Core.Configuration;

/// <summary>
/// The configuration for a table
/// </summary>
public class TableConfig {
    /// [GENERATED] The unique identifier for the table (usually the name of the model)
    public required string Identifier { get; init; }
    
    /// [GENERATED] The configurations for the properties of the model
    public IList<PropertyConfig> Properties { get; set; } = new List<PropertyConfig>();

    /// [GENERATED] The type of the model
    public required Type TableType { get; set; }

    /// [GENERATED] The type identifier for the repository
    public required Type RepositoryType { get; set; }
    
    /// [GENERATED] the url of the table page
    public required string Route { get; set; }
    
    /// [GENERATED] The displayed name of the table
    public required string DisplayName { get; set; }
    
    /// A short description for the table
    public string? Description { get; set; }
    
    /// [GENERATED] The place (from top to bottom) that the table will appear in on the sidebar
    public int OrderIndex { get; set; }
    
    /// [GENERATED] The identifier of the property that should be displayed if the model is used as a relation
    public string? PreferredProperty { get; set; }
    
    internal TableConfig() {}
}