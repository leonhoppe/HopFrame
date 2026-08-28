using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public Type TableType { get; set; } = null!;

    /// [GENERATED] The type identifier for the repository
    [JsonIgnore]
    public Type RepositoryType { get; set; } = null!;

    /// [GENERATED] If the type of the table is an <see cref="IDictionary{string,object}"/> it is treated as a virtual table
    public bool IsDictionary { get; set; }
    
    /// [GENERATED] the url of the table page
    public required string Route { get; set; }
    
    /// [GENERATED] The displayed name of the table
    public required string DisplayName { get; set; }
    
    /// A short description for the table
    public string? Description { get; set; }
    
    /// [GENERATED] The place (from top to bottom) that the table will appear in on the sidebar. By default, it's incremented by 10
    public int OrderIndex { get; set; }
    
    /// [GENERATED] The identifier of the property that should be displayed if the model is used as a relation
    public string? PreferredProperty { get; set; }

    /// The claim the user needs to access the table
    [JsonIgnore]
    public string? ViewClaim { get; set; }

    /// The claim the user needs to edit, delete or create entries in the table
    [JsonIgnore]
    public string? EditClaim { get; set; }

    /// If set, the table will be displayed under a specific category
    public string? Category { get; set; }
}