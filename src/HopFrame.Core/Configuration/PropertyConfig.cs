namespace HopFrame.Core.Configuration;

/**
 * The configuration for a single property
 */
public class PropertyConfig {
    /** The unique identifier for the property (usually the real property name in the model) */
    public required string Identifier { get; init; }
    
    /** The displayed name of the Property */
    public required string DisplayName { get; set; }
    
    /** The type of the property */
    public required Type Type { get; set; }
    
    /** Determines if the property will appear in the table */
    public bool Listable { get; set; } = true;
    
    /** Determines if the table can be sorted by the property */
    public bool Sortable { get; set; } = true;
    
    /** Determines if the table can be searched by the property */
    public bool Searchable { get; set; } = true;
    
    /**
     * Determines if the value of the property can be edited
     * (if true the value can still be set during creation)
     */
    public bool Editable { get; set; } = true;
    
    /** Determines if the property is visible in the creation or edit dialog */
    public bool Creatable { get; set; } = true;
    
    /** Determines if the actual value should be displayed (useful for passwords) */
    public bool DisplayValue { get; set; } = true;

    /** The place (from left to right) that the property will appear in the table and editor */
    public int OrderIndex { get; set; }
    
    internal PropertyConfig() {}
}