using System.Linq.Expressions;
using HopFrame.Core.Configuration;
using HopFrame.Core.Helpers;

namespace HopFrame.Core.Configurators;

/// <summary>
/// The configurator for the <see cref="TableConfig"/>
/// </summary>
public class TableConfigurator<TModel>(TableConfig config) where TModel : class {
    /// The internal config that is modified
    public TableConfig Config { get; } = config;

    /// <inheritdoc cref="TableConfig.Route"/>
    public TableConfigurator<TModel> SetRoute(string route) {
        Config.Route = route;
        return this;
    }

    /// <inheritdoc cref="TableConfig.DisplayName"/>
    public TableConfigurator<TModel> SetDisplayName(string displayName) {
        Config.DisplayName = displayName;
        return this;
    }

    /// <inheritdoc cref="TableConfig.Description"/>
    public TableConfigurator<TModel> SetDescription(string description) {
        Config.Description = description;
        return this;
    }

    /// <inheritdoc cref="TableConfig.OrderIndex"/>
    public TableConfigurator<TModel> SetOrderIndex(int index) {
        Config.OrderIndex = index;
        return this;
    }

    /// <summary>
    /// Add a new property that's not inferred by real properties of the model. Please ensure to invoke
    /// <see cref="PropertyConfigurator{TModel,TProp}.SetFormatter"/> and <see cref="PropertyConfigurator{TModel,TProp}.SetParser"/>
    /// otherwise an error will be thrown when trying to get or set the value of the property.
    /// </summary>
    /// <param name="name">The name of the property</param>
    /// <typeparam name="TProp">The type of the property</typeparam>
    public PropertyConfigurator<TModel, TProp> AddProperty<TProp>(string name) {
        var prop = ConfigurationHelper.InitializeProperty(Config, typeof(TProp), name, null);
        Config.Properties.Add(prop);
        return new PropertyConfigurator<TModel, TProp>(prop);
    }

    /// <summary>
    /// Returns the configurator for a property
    /// </summary>
    /// <typeparam name="TProp">The type of the property</typeparam>
    /// <exception cref="ArgumentException">Is thrown, when no property was found</exception>
    public PropertyConfigurator<TModel, TProp> Property<TProp>(string identifier) {
        var prop = Config.Properties
            .FirstOrDefault(p => p.Identifier == identifier);

        if (prop is null)
            throw new ArgumentException($"No attribute '{identifier}' found in '{Config.Identifier}'!");
        
        return new PropertyConfigurator<TModel, TProp>(prop);
    }

    /// <inheritdoc cref="Property(string)"/>
    public PropertyConfigurator<TModel, TProp> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression) {
        var propertyName = ExpressionHelper.GetPropertyInfo(propertyExpression).Name;
        var prop = Config.Properties.FirstOrDefault(p => p.Identifier == propertyName);
        
        if (prop is null)
            throw new ArgumentException($"No attribute '{propertyName}' found in '{Config.Identifier}'!");
        
        return new PropertyConfigurator<TModel, TProp>(prop);
    }
    
    /// <inheritdoc cref="TableConfig.PreferredProperty"/>
    public TableConfigurator<TModel> SetPreferredProperty(string identifier) {
        var prop = Config.Properties.FirstOrDefault(p => p.Identifier == identifier);
        
        if (prop is null)
            throw new ArgumentException($"No attribute '{identifier}' found in '{Config.Identifier}'!");

        Config.PreferredProperty = prop.Identifier;
        return this;
    }

    /// <inheritdoc cref="TableConfig.PreferredProperty"/>
    public TableConfigurator<TModel> SetPreferredProperty(Expression<Func<TModel, object?>> propertyExpression) {
        var propertyName = ExpressionHelper.GetPropertyInfo(propertyExpression).Name;
        var prop = Config.Properties.FirstOrDefault(p => p.Identifier == propertyName);
        
        if (prop is null)
            throw new ArgumentException($"No attribute '{propertyName}' found in '{Config.Identifier}'!");

        Config.PreferredProperty = prop.Identifier;
        return this;
    }
}