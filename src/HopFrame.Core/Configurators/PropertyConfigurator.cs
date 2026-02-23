using HopFrame.Core.Configuration;

namespace HopFrame.Core.Configurators;

/// <summary>
/// The configurator for the <see cref="PropertyConfig"/>
/// </summary>
public class PropertyConfigurator(PropertyConfig config) {
    /// The internal config that is modified
    public PropertyConfig Config { get; } = config;

    /// <inheritdoc cref="PropertyConfig.DisplayName" />
    public PropertyConfigurator SetDisplayName(string displayName) {
        Config.DisplayName = displayName;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Listable" />
    public PropertyConfigurator Listable(bool listable) {
        Config.Listable = listable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Sortable" />
    public PropertyConfigurator Sortable(bool sortable) {
        Config.Sortable = sortable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Searchable" />
    public PropertyConfigurator Searchable(bool searchable) {
        Config.Searchable = searchable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Editable" />
    public PropertyConfigurator Editable(bool editable) {
        Config.Editable = editable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.Creatable" />
    public PropertyConfigurator Creatable(bool creatable) {
        Config.Creatable = creatable;
        return this;
    }

    /// <inheritdoc cref="PropertyConfig.OrderIndex" />
    public PropertyConfigurator SetOrderIndex(int index) {
        Config.OrderIndex = index;
        return this;
    }

    /// <summary>
    /// Sets the property type. The predefined modifiers (like nullable) persist.
    /// If the property is a list or any other generic type, please use the enumerated type.
    /// </summary>
    public PropertyConfigurator SetType(PropertyType type) {
        Config.PropertyType = (PropertyType)(((byte)Config.PropertyType & 0xF0) | ((byte)type & 0x0F));
        return this;
    }
}