using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Classes;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal sealed class AdminPageGenerator<TModel> : IAdminPageGenerator<TModel>, IGenerator<AdminPage<TModel>> {

    public readonly AdminPage<TModel> Page;
    private readonly Dictionary<string, object> _propertyGenerators;

    public AdminPageGenerator() {
        Page = new AdminPage<TModel> {
            Permissions = new AdminPagePermissions(),
            ModelType = typeof(TModel)
        };
        _propertyGenerators = new Dictionary<string, object>();

        var type = typeof(TModel);
        var properties = type.GetProperties();
        var generatorType = typeof(AdminPropertyGenerator<>);
        
        foreach (var property in properties) {
            var attributes = property.GetCustomAttributes(false);
            var genericType = generatorType.MakeGenericType(property.PropertyType);
            
            var generator = Activator.CreateInstance(genericType, [property.Name, property.PropertyType]);

            var method = genericType
                .GetMethod(nameof(AdminPropertyGenerator<object>.ApplyConfigurationFromAttributes))?
                .MakeGenericMethod(type);
            method?.Invoke(generator, [this, attributes, property]);
            
            _propertyGenerators.Add(property.Name, generator);
        }
    }

    public IAdminPageGenerator<TModel> Title(string title) {
        Page.Title = title;
        Page.Url ??= title.ToLower();
        return this;
    }

    public IAdminPageGenerator<TModel> Description(string description) {
        Page.Description = description;
        return this;
    }

    public IAdminPageGenerator<TModel> Url(string url) {
        Page.Url = url;
        return this;
    }

    public IAdminPageGenerator<TModel> ViewPermission(string permission) {
        Page.Permissions.View = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> CreatePermission(string permission) {
        Page.Permissions.Create = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> UpdatePermission(string permission) {
        Page.Permissions.Update = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> DeletePermission(string permission) {
        Page.Permissions.Delete = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> ShowCreateButton(bool show) {
        Page.ShowCreateButton = show;
        return this;
    }

    public IAdminPageGenerator<TModel> ShowDeleteButton(bool show) {
        Page.ShowDeleteButton = show;
        return this;
    }

    public IAdminPageGenerator<TModel> ShowUpdateButton(bool show) {
        Page.ShowUpdateButton = show;
        return this;
    }

    public IAdminPageGenerator<TModel> DefaultSort<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, ListSortDirection direction) {
        var property = GetPropertyInfo(propertyExpression);
        
        Page.DefaultSortPropertyName = property.Name;
        Page.DefaultSortDirection = direction;
        return this;
    }

    public IAdminPageGenerator<TModel> ConfigureRepository<TRepository>() where TRepository : ModelRepository<TModel> {
        Page.RepositoryProvider = typeof(TRepository);
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Property<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression) {
        var property = GetPropertyInfo(propertyExpression);

        if (_propertyGenerators.TryGetValue(property.Name, out var propertyGenerator))
            return propertyGenerator as AdminPropertyGenerator<TProperty>;
        
        var generator = Activator.CreateInstance(typeof(AdminPropertyGenerator<TProperty>), new { property.Name, property.PropertyType }) as AdminPropertyGenerator<TProperty>;
        generator?.ApplyConfigurationFromAttributes(this, property.GetCustomAttributes(false), property);
        _propertyGenerators.Add(property.Name, generator);
        
        return generator;
    }

    public IAdminPageGenerator<TModel> ListingProperty<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression) {
        var property = GetPropertyInfo(propertyExpression);
        Page.ListingProperty = property.Name;
        return this;
    }

    public AdminPage<TModel> Compile() {
        var properties = new List<AdminPageProperty>();
        
        foreach (var generator in _propertyGenerators.Values) {
            var method = generator.GetType().GetMethod(nameof(AdminPropertyGenerator<object>.Compile));
            var prop = method?.Invoke(generator, []) as AdminPageProperty;
            properties.Add(prop);
        }

        Page.Properties = properties;
        
        return Page;
    }

    public static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda) {
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
    
    public void ApplyConfigurationFromAttributes(object[] attributes) {
        if (attributes.Any(a => a is AdminNameAttribute)) {
            var attribute = attributes.Single(a => a is AdminNameAttribute) as AdminNameAttribute;
            Title(attribute?.Name);
        }
            
        if (attributes.Any(a => a is AdminDescriptionAttribute)) {
            var attribute = attributes.Single(a => a is AdminDescriptionAttribute) as AdminDescriptionAttribute;
            Description(attribute?.Description);
        }

        if (attributes.Any(a => a is AdminUrlAttribute)) {
            var attribute = attributes.Single(a => a is AdminUrlAttribute) as AdminUrlAttribute;
            Url(attribute?.Url);
        }

        if (attributes.Any(a => a is AdminPermissionsAttribute)) {
            var attribute = attributes.Single(a => a is AdminPermissionsAttribute) as AdminPermissionsAttribute;
            CreatePermission(attribute?.Permissions.Create);
            UpdatePermission(attribute?.Permissions.Update);
            ViewPermission(attribute?.Permissions.View);
            DeletePermission(attribute?.Permissions.Delete);
        }

        if (attributes.Any(a => a is AdminButtonConfigAttribute)) {
            var attribute = attributes.Single(a => a is AdminButtonConfigAttribute) as AdminButtonConfigAttribute;
            ShowCreateButton(attribute?.ShowCreateButton == true);
            ShowUpdateButton(attribute?.ShowUpdateButton == true);
            ShowDeleteButton(attribute?.ShowDeleteButton == true);
        }
    }
}