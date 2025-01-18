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

public class TableConfig<TModel>(TableConfig config) {
    public TableConfig InnerConfig { get; } = config;

    public TableConfig<TModel> Ignore() {
        InnerConfig.Ignored = true;
        return this;
    }

    public PropertyConfig<TProp> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression) {
        var info = GetPropertyInfo(propertyExpression);
        var prop = InnerConfig.Properties
            .Single(prop => prop.Info.Name == info.Name);
        return new PropertyConfig<TProp>(prop);
    }
    
    public TableConfig<TModel> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression, Action<PropertyConfig<TProp>> configurator) {
        var prop = Property(propertyExpression);
        configurator.Invoke(prop);
        return this;
    }

    public PropertyConfig<string> AddListingProperty(string name, Func<TModel, IServiceProvider, string> template) {
        var prop = new PropertyConfig(InnerConfig.Properties.First().Info, InnerConfig, InnerConfig.Properties.Count);
        prop.Name = name;
        prop.IsListingProperty = true;
        prop.Formatter = (obj, provider) => template.Invoke((TModel)obj, provider);
        InnerConfig.Properties.Add(prop);
        return new PropertyConfig<string>(prop);
    }
    
    public TableConfig<TModel> AddListingProperty(string name, Func<TModel, IServiceProvider, string> template, Action<PropertyConfig<string>> configurator) {
        var prop = AddListingProperty(name, template);
        configurator.Invoke(prop);
        return this;
    }

    public TableConfig<TModel> SetDisplayName(string name) {
        InnerConfig.DisplayName = name;
        return this;
    }

    public TableConfig<TModel> SetDescription(string description) {
        InnerConfig.Description = description;
        return this;
    }

    public TableConfig<TModel> SetOrderIndex(int index) {
        InnerConfig.Order = index;
        return this;
    }

    public TableConfig<TModel> SetViewPolicy(string policy) {
        InnerConfig.ViewPolicy = policy;
        return this;
    }
    
    public TableConfig<TModel> SetUpdatePolicy(string policy) {
        InnerConfig.UpdatePolicy = policy;
        return this;
    }
    
    public TableConfig<TModel> SetCreatePolicy(string policy) {
        InnerConfig.CreatePolicy = policy;
        return this;
    }
    
    public TableConfig<TModel> SetDeletePolicy(string policy) {
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
