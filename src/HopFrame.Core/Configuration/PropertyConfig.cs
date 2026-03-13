namespace HopFrame.Core.Configuration;

/// <summary>
/// The configuration for a single property
/// </summary>
public class PropertyConfig {
    /// [GENERATED] The unique identifier for the property (usually the real property name in the model)
    public required string Identifier { get; init; }
    
    /// [GENERATED] The displayed name of the Property
    public required string DisplayName { get; set; }
    
    /// [GENERATED] The real type of the property
    public required Type Type { get; set; }
    
    /// [GENERATED] The underlying type of the relation object
    public Type? RelationType { get; set; }

    /// [GENERATED] The type as wich the property should be treated
    public required PropertyType PropertyType { get; set; }

    /// The range of lines the input field grows to (only applies to text areas)
    public Range SizeRange { get; set; } = 1..5;
    
    /// Determines if the property will appear in the table
    public bool Listable { get; set; } = true;
    
    /// Determines if the table can be sorted by the property
    public bool Sortable { get; set; } = true;
    
    /// Determines if the table can be searched by the property
    public bool Searchable { get; set; } = true;
    
    /// <summary>
    /// Determines if the value of the property can be edited<br/>
    /// (if true the value can still be set during creation)
    /// </summary>
    public bool Editable { get; set; } = true;
    
    /// Determines if the property is visible in the creation or edit dialog
    public bool Creatable { get; set; } = true;

    /// Determines if the property is visible in the creator or editor
    public bool VisibleInEditor { get; set; } = true;

    /// [GENERATED] The place (from left to right) that the property will appear in the table and editor
    public int OrderIndex { get; set; }

    /// [GENERATED] The table that owns this property
    public TableConfig Table { get; init; } = null!;

    /// If set, the function determines the value that should be displayed
    public Func<object, object?>? Getter { get; set; }

    /// If set, the function is executed to format the user entered value for the property
    public Action<object, object?>? Setter { get; set; }
    
    /// <summary>
    /// If set, the function is executed to validate if the provided value is valid.<br/>
    /// It should return a list of errors that were found, or an empty list if no error was found
    /// </summary>
    public Func<object, IEnumerable<string>>? Validator { get; set; }
}

/// <summary>
/// Used to distinguish between different input types in the frontend. <br/>
/// Binary Format: First byte is used for additional properties, second byte identifies the real type
/// </summary>
[Flags]
public enum PropertyType : byte {
    /// Used together with another type to indicate that the value can be null
    Nullable = 0b10000000,
    
    /// Used together with another type to indicate that the property is a relation
    Relation = 0b01000000,
    
    /// Used together with another type to indicate that the value is enumerable
    List = 0b00100000,
    
    /// Indicates that the value is numeric
    Numeric = 0x01,
    
    /// Indicates that the value is a boolean
    Boolean = 0x02,
    
    /// Indicates that the value is a timestamp
    DateTime = 0x03,
    
    /// Indicates that the value is a date
    DateOnly = 0x04,
    
    /// Indicates that the value is a time of day
    TimeOnly = 0x05,
    
    /// Indicates that the value is a list of fixed values
    Enum = 0x06,
    
    /// Indicates that the value is a string
    Text = 0x07,
    
    /// Indicates that the value is an email
    Email = 0x08,
    
    /// Indicates that the value is a long string
    TextArea = 0x09,
    
    /// Indicates that the value should be hidden
    Password = 0x0A,
    
    /// Indicates that the value is a phone number
    PhoneNumber = 0x0B
}
