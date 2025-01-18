using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace HopFrame.Core.Config;

public class TableConfig {
    public Type TableType { get; }
    public string PropertyName { get; }
    public string DisplayName { get; set; }
    public string? Description { get; set; }
    public DbContextConfig ContextConfig { get; }
    public bool Ignored { get; set; }
    public int Order { get; set; }
    internal bool Seeded { get; set; }

    public string? ViewPolicy { get; set; }
    public string? CreatePolicy { get; set; }
    public string? UpdatePolicy { get; set; }
    public string? DeletePolicy { get; set; }
    
    public List<PropertyConfig> Properties { get; } = new();

    public TableConfig(DbContextConfig config, Type tableType, string propertyName, int nthTable) {
        TableType = tableType;
        PropertyName = propertyName;
        ContextConfig = config;
        DisplayName = PropertyName;
        Order = nthTable;

        var properties = tableType.GetProperties();
        for (var i = 0; i < properties.Length; i++) {
            var info = properties[i];
            var propConfig = new PropertyConfig(info, this, i);

            if (info.GetCustomAttributes(true).Any(a => a is DatabaseGeneratedAttribute)) {
                propConfig.Creatable = false;
                propConfig.Editable = false;
            }

            if (info.GetCustomAttributes(true).Any(a => a is KeyAttribute)) {
                propConfig.Editable = false;
            }

            Properties.Add(propConfig);
        }
    }
}

/// <summary>
/// A helper class for editing the <see cref="TableConfig"/>
/// </summary>
public class TableConfigurator<TModel>(TableConfig config) {
    
    /// <summary>
    /// The Internal property configuration that's modified by the helper functions
    /// </summary>
    public TableConfig InnerConfig { get; } = config;

    /// <summary>
    /// Determines if the table should be ignored in the admin ui
    /// </summary>
    public TableConfigurator<TModel> Ignore(bool ignore) {
        InnerConfig.Ignored = ignore;
        return this;
    }

    /// <summary>
    /// Configures the property of the table
    /// </summary>
    /// <param name="propertyExpression">Used for determining the property</param>
    /// <returns>The configurator for the specified property</returns>
    /// <seealso cref="PropertyConfigurator{TProp}"/>
    public PropertyConfigurator<TProp> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression) {
        var info = GetPropertyInfo(propertyExpression);
        var prop = InnerConfig.Properties
            .Single(prop => prop.Info.Name == info.Name);
        return new PropertyConfigurator<TProp>(prop);
    }
    
    /// <summary>
    /// Configures the property of the table using the provided configurator
    /// </summary>
    /// <param name="propertyExpression">Used for determining the property</param>
    /// <param name="configurator">Used for configuring the property</param>
    /// <seealso cref="PropertyConfigurator{TProp}"/>
    public TableConfigurator<TModel> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression, Action<PropertyConfigurator<TProp>> configurator) {
        var prop = Property(propertyExpression);
        configurator.Invoke(prop);
        return this;
    }

    /// <summary>
    /// Adds a virtual property to the table view (this property will not appear in the editor)
    /// </summary>
    /// <param name="name">The name of the virtual property</param>
    /// <param name="template">The template used for generating the property value</param>
    /// <returns>The configurator for the virtual property</returns>
    /// <seealso cref="PropertyConfigurator{TProp}"/>
    public PropertyConfigurator<string> AddVirtualProperty(string name, Func<TModel, IServiceProvider, string> template) {
        var prop = new PropertyConfig(InnerConfig.Properties.First().Info, InnerConfig, InnerConfig.Properties.Count);
        prop.Name = name;
        prop.IsListingProperty = true;
        prop.Formatter = (obj, provider) => template.Invoke((TModel)obj, provider);
        InnerConfig.Properties.Add(prop);
        return new PropertyConfigurator<string>(prop);
    }
    
    /// <summary>
    /// Adds a virtual property to the table view and configures it using the provided configurator (this property will not appear in the editor)
    /// </summary>
    /// <param name="name">The name of the virtual property</param>
    /// <param name="template">The template used for generating the property value</param>
    /// <param name="configurator">Used for configuring the virtual property</param>
    /// <seealso cref="PropertyConfigurator{TProp}"/>
    public TableConfigurator<TModel> AddVirtualProperty(string name, Func<TModel, IServiceProvider, string> template, Action<PropertyConfigurator<string>> configurator) {
        var prop = AddVirtualProperty(name, template);
        configurator.Invoke(prop);
        return this;
    }

    /// <summary>
    /// Determines the name for the table used in the admin ui and url for the table page
    /// </summary>
    public TableConfigurator<TModel> SetDisplayName(string name) {
        InnerConfig.DisplayName = name;
        return this;
    }

    /// <summary>
    /// Determines the description displayed in the admin ui
    /// </summary>
    public TableConfigurator<TModel> SetDescription(string description) {
        InnerConfig.Description = description;
        return this;
    }

    /// <summary>
    /// Determines the order index for the table in the admin ui
    /// </summary>
    /// <seealso cref="PropertyConfigurator{TProp}.SetOrderIndex"/>
    public TableConfigurator<TModel> SetOrderIndex(int index) {
        InnerConfig.Order = index;
        return this;
    }

    /// <summary>
    /// Determines the policy needed by a user in order to view the table
    /// </summary>
    public TableConfigurator<TModel> SetViewPolicy(string policy) {
        InnerConfig.ViewPolicy = policy;
        return this;
    }
    
    /// <summary>
    /// Determines the policy needed by a user in order to edit the entries
    /// </summary>
    public TableConfigurator<TModel> SetUpdatePolicy(string policy) {
        InnerConfig.UpdatePolicy = policy;
        return this;
    }
    
    /// <summary>
    /// Determines the policy needed by a user in order to create entries
    /// </summary>
    public TableConfigurator<TModel> SetCreatePolicy(string policy) {
        InnerConfig.CreatePolicy = policy;
        return this;
    }
    
    /// <summary>
    /// Determines the policy needed by a user in order to delete entries
    /// </summary>
    public TableConfigurator<TModel> SetDeletePolicy(string policy) {
        InnerConfig.DeletePolicy = policy;
        return this;
    }
    
    internal static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda) {
        if (propertyLambda.Body is not MemberExpression member) {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
        }

        if (member.Member is not PropertyInfo propInfo) {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
        }

        Type type = typeof(TSource);
        if (propInfo.ReflectedType != null && type != propInfo.ReflectedType &&
            !type.IsSubclassOf(propInfo.ReflectedType)) {
            throw new ArgumentException($"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
        }
        
        if (propInfo.Name is null)
            throw new ArgumentException($"Expression '{propertyLambda}' refers a not existing property.");

        return propInfo;
    }

    
}
