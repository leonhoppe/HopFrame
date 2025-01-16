using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace HopFrame.Core.Config;

public class TableConfig {
    public Type TableType { get; }
    public string PropertyName { get; }
    public string DisplayName { get; set; }
    public DbContextConfig ContextConfig { get; }
    public bool Ignored { get; set; }
    internal bool Seeded { get; set; }

    public List<PropertyConfig> Properties { get; } = new();

    public TableConfig(DbContextConfig config, Type tableType, string propertyName) {
        TableType = tableType;
        PropertyName = propertyName;
        ContextConfig = config;
        DisplayName = PropertyName;

        foreach (var info in tableType.GetProperties()) {
            var propConfig = new PropertyConfig(info, this);

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

    public TableConfig<TModel> SetDisplayName(string name) {
        InnerConfig.DisplayName = name;
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
