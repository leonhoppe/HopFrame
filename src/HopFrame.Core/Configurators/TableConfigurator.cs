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

    /// Returns the configurator for a property
    public PropertyConfigurator Property(string identifier) {
        var prop = Config.Properties
            .FirstOrDefault(p => p.Identifier == identifier);

        if (prop is null)
            throw new ArgumentException($"No attribute '{identifier}' found in '{Config.Identifier}'!");
        
        return new PropertyConfigurator(prop);
    }

    /// <inheritdoc cref="Property"/>
    public PropertyConfigurator Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression) {
        var propertyName = ExpressionHelper.GetPropertyInfo(propertyExpression).Name;
        var prop = Config.Properties.FirstOrDefault(p => p.Identifier == propertyName);
        
        if (prop is null)
            throw new ArgumentException($"No attribute '{propertyName}' found in '{Config.Identifier}'!");
        
        return new PropertyConfigurator(prop);
    }
}