using HopFrame.Core.Configuration;

namespace HopFrame.Core.Configurators;

/// <summary>
/// The configurator for the <see cref="PropertyConfig"/>
/// </summary>
public class PropertyConfigurator<TModel, TProp>(PropertyConfig config) where TModel : class {
    /// The internal config that is modified
    public PropertyConfig Config { get; } = config;

    /// <inheritdoc cref="PropertyConfig.DisplayName" />
    public PropertyConfigurator<TModel, TProp> SetDisplayName(string displayName) {
        Config.DisplayName = displayName;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Listable" />
    public PropertyConfigurator<TModel, TProp> Listable(bool listable) {
        Config.Listable = listable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Sortable" />
    public PropertyConfigurator<TModel, TProp> Sortable(bool sortable) {
        Config.Sortable = sortable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Searchable" />
    public PropertyConfigurator<TModel, TProp> Searchable(bool searchable) {
        Config.Searchable = searchable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Editable" />
    public PropertyConfigurator<TModel, TProp> Editable(bool editable) {
        Config.Editable = editable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Creatable" />
    public PropertyConfigurator<TModel, TProp> Creatable(bool creatable) {
        Config.Creatable = creatable;

        if (creatable == false)
            Config.Editable = false;
        
        return this;
    }
    
    /// <inheritdoc cref="PropertyConfig.VisibleInEditor" />
    public PropertyConfigurator<TModel, TProp> VisibleInEditor(bool visible) {
        Config.VisibleInEditor = visible;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.OrderIndex" />
    public PropertyConfigurator<TModel, TProp> SetOrderIndex(int index) {
        Config.OrderIndex = index;
        return this;
    }
    
    /// <inheritdoc cref="PropertyConfig.SizeRange" />
    public PropertyConfigurator<TModel, TProp> SetSizeRange(Range range) {
        Config.SizeRange = range;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Getter" />
    public PropertyConfigurator<TModel, TProp> SetFormatter(Func<TModel, TProp> formatter) {
        Config.Getter = model => formatter.Invoke((TModel)model);
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Setter" />
    public PropertyConfigurator<TModel, TProp> SetParser(Action<TModel, object?> parser) {
        Config.Setter = (model, value) => parser.Invoke((TModel)model, value);
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Validator" />
    public PropertyConfigurator<TModel, TProp> SetValidator(Func<TProp, IEnumerable<string>> validator) {
        Config.Validator = value => validator.Invoke((TProp)value);
        return this;
    }

    /// <summary>
    /// Sets the property type. The predefined modifiers (like nullable) persist.
    /// If the property is a list or any other generic type, please use the enumerated type.
    /// </summary>
    public PropertyConfigurator<TModel, TProp> SetType(PropertyType type) {
        Config.PropertyType = (PropertyType)(((byte)Config.PropertyType & 0xF0) | ((byte)type & 0x0F));
        return this;
    }

    /// <summary>
    /// Sets the property type including the provided modifiers (like nullable).
    /// If the property is a list or any other generic type, please use the enumerated type.
    /// </summary>
    public PropertyConfigurator<TModel, TProp> SetTypeRaw(PropertyType type) {
        Config.PropertyType = type;
        return this;
    }

    /// <summary>
    /// Forces the property to be a relation to another table
    /// </summary>
    /// <param name="relationTable">The table to relate to</param>
    public PropertyConfigurator<TModel, TProp> IsRelation<TRelation>(TableConfigurator<TRelation> relationTable) where TRelation : class {
        Config.PropertyType |= PropertyType.Relation;
        Config.RelationTable = relationTable.Config.Identifier;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.DropdownOptions" />
    public PropertyConfigurator<TModel, TProp> AsDropdown(params TProp[] options) {
        Config.DropdownOptions = [.. options.Cast<object>()];
        return this;
    }
}