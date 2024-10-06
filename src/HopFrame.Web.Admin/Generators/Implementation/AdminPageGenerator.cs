using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Members;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal sealed class AdminPageGenerator<TModel> : IAdminPageGenerator<TModel>, IGenerator<AdminPage<TModel>> {

    private readonly AdminPage<TModel> _page;
    private readonly IDictionary<string, AdminPropertyGenerator> _propertyGenerators;

    public AdminPageGenerator() {
        _page = new AdminPage<TModel> {
            Permissions = new AdminPagePermissions()
        };
        _propertyGenerators = new Dictionary<string, AdminPropertyGenerator>();

        var type = typeof(TModel);
        var properties = type.GetProperties();
        
        foreach (var property in properties) {
            var attributes = property.GetCustomAttributes(false);
            
            var generator = Activator.CreateInstance(typeof(AdminPropertyGenerator), [property.Name, property.PropertyType]) as AdminPropertyGenerator;

            ApplyConfigurationFromAttributes(generator, attributes, property);
            
            _propertyGenerators.Add(property.Name, generator);
        }
    }

    public AdminPageGenerator(string title) : this() {
        Title(title);
    }

    public IAdminPageGenerator<TModel> Title(string title) {
        _page.Title = title;
        _page.Url ??= title.ToLower();
        return this;
    }

    public IAdminPageGenerator<TModel> Description(string description) {
        _page.Description = description;
        return this;
    }

    public IAdminPageGenerator<TModel> Url(string url) {
        _page.Url = url;
        return this;
    }

    public IAdminPageGenerator<TModel> ViewPermission(string permission) {
        _page.Permissions.View = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> CreatePermission(string permission) {
        _page.Permissions.Create = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> UpdatePermission(string permission) {
        _page.Permissions.Update = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> DeletePermission(string permission) {
        _page.Permissions.Delete = permission;
        return this;
    }

    public IAdminPageGenerator<TModel> ShowCreateButton(bool show) {
        _page.ShowCreateButton = show;
        return this;
    }

    public IAdminPageGenerator<TModel> ShowDeleteButton(bool show) {
        _page.ShowDeleteButton = show;
        return this;
    }

    public IAdminPageGenerator<TModel> ShowUpdateButton(bool show) {
        _page.ShowUpdateButton = show;
        return this;
    }

    public IAdminPageGenerator<TModel> DefaultSort<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, ListSortDirection direction) {
        var property = GetPropertyInfo(propertyExpression);
        
        _page.DefaultSortPropertyName = property.Name;
        _page.DefaultSortDirection = direction;
        return this;
    }

    public IAdminPageGenerator<TModel> ConfigureRepository<TRepository>() where TRepository : IModelRepository<TModel> {
        _page.RepositoryProvider = typeof(TRepository);
        return this;
    }

    public IAdminPropertyGenerator Property<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression) {
        var property = GetPropertyInfo(propertyExpression);

        if (_propertyGenerators.TryGetValue(property.Name, out var propertyGenerator))
            return propertyGenerator;
        
        var generator = Activator.CreateInstance(typeof(AdminPropertyGenerator), new { property.Name, property.PropertyType }) as AdminPropertyGenerator;
        ApplyConfigurationFromAttributes(generator, property.GetCustomAttributes(false), property);
        _propertyGenerators.Add(property.Name, generator);
        
        return generator;
    }

    public AdminPage<TModel> Compile() {
        var properties = new List<AdminPageProperty>();
        
        foreach (var generator in _propertyGenerators.Values){
            properties.Add(generator.Compile());
        }

        _page.Properties = properties;
        
        return _page;
    }

    private void ApplyConfigurationFromAttributes(AdminPropertyGenerator generator, object[] attributes, PropertyInfo property) {
        if (attributes.Any(a => a is KeyAttribute)) {
            _page.DefaultSortPropertyName = property.Name;
            generator.Bold();
            generator.Editable(false);
        }

        if (attributes.Any(a => a is AdminUnsortableAttribute))
            generator.Sortable(false);

        if (attributes.Any(a => a is AdminUneditableAttribute))
            generator.Editable(false);

        if (attributes.Any(a => a is AdminBoldAttribute))
            generator.Bold();
        
        if (attributes.Any(a => a is AdminIgnoreAttribute)) {
            var attribute = attributes.Single(a => a is AdminIgnoreAttribute) as AdminIgnoreAttribute;
            generator.DisplayInListing(false);
            generator.Sortable(false);
            generator.Ignore(attribute?.OnlyForListing == false);
        }

        if (attributes.Any(a => a is AdminHideValueAttribute))
            generator.DisplayValueWhileEditing(false);

        if (attributes.Any(a => a is AdminNameAttribute)) {
            var attribute = attributes.Single(a => a is AdminNameAttribute) as AdminNameAttribute;
            generator.DisplayName(attribute?.Name);
        }
            
        if (attributes.Any(a => a is AdminDescriptionAttribute)) {
            var attribute = attributes.Single(a => a is AdminDescriptionAttribute) as AdminDescriptionAttribute;
            generator.Description(attribute?.Description);
        }

        if (attributes.Any(a => a is AdminPrefixAttribute)) {
            var attribute = attributes.Single(a => a is AdminPrefixAttribute) as AdminPrefixAttribute;
            generator.Prefix(attribute?.Prefix);
        }
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
    
}