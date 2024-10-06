using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Classes;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal sealed class AdminPageGenerator<TModel> : IAdminPageGenerator<TModel>, IGenerator<AdminPage<TModel>> {

    public readonly AdminPage<TModel> Page;
    private readonly IDictionary<string, AdminPropertyGenerator> _propertyGenerators;

    public AdminPageGenerator() {
        Page = new AdminPage<TModel> {
            Permissions = new AdminPagePermissions()
        };
        _propertyGenerators = new Dictionary<string, AdminPropertyGenerator>();

        var type = typeof(TModel);
        var properties = type.GetProperties();
        
        foreach (var property in properties) {
            var attributes = property.GetCustomAttributes(false);
            
            var generator = Activator.CreateInstance(typeof(AdminPropertyGenerator), [property.Name, property.PropertyType]) as AdminPropertyGenerator;
            generator?.ApplyConfigurationFromAttributes(this, attributes, property);
            
            _propertyGenerators.Add(property.Name, generator);
        }
    }

    public AdminPageGenerator(string title) : this() {
        Title(title);
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

    public IAdminPageGenerator<TModel> ConfigureRepository<TRepository>() where TRepository : IModelRepository<TModel> {
        Page.RepositoryProvider = typeof(TRepository);
        return this;
    }

    public IAdminPropertyGenerator Property<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression) {
        var property = GetPropertyInfo(propertyExpression);

        if (_propertyGenerators.TryGetValue(property.Name, out var propertyGenerator))
            return propertyGenerator;
        
        var generator = Activator.CreateInstance(typeof(AdminPropertyGenerator), new { property.Name, property.PropertyType }) as AdminPropertyGenerator;
        generator?.ApplyConfigurationFromAttributes(this, property.GetCustomAttributes(false), property);
        _propertyGenerators.Add(property.Name, generator);
        
        return generator;
    }

    public AdminPage<TModel> Compile() {
        var properties = new List<AdminPageProperty>();
        
        foreach (var generator in _propertyGenerators.Values){
            properties.Add(generator.Compile());
        }

        Page.Properties = properties;
        
        return Page;
    }

    private static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda) {
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