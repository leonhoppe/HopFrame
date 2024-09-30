using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Classes;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal class AdminContextGenerator : IAdminContextGenerator {

    private readonly IDictionary<Type, object> _adminPages = new Dictionary<Type, object>();

    public IAdminPageGenerator<TModel> Page<TModel>() {
        if (_adminPages.TryGetValue(typeof(TModel), out var pageGenerator))
            return pageGenerator as IAdminPageGenerator<TModel>;

        var generator = Activator.CreateInstance(typeof(IAdminPageGenerator<TModel>)) as IAdminPageGenerator<TModel>;
        
        ApplyConfigurationFromAttributes(generator, typeof(TModel).GetCustomAttributes(false));
        
        _adminPages.Add(typeof(TModel), generator);

        return generator;
    }

    public AdminPage<TModel> CompilePage<TModel>() {
        var generator = _adminPages[typeof(TModel)];
        if (generator is null) return null;

        return (generator as AdminPageGenerator<TModel>)?.Compile();
    }

    public TContext CompileContext<TContext>() where TContext : AdminPagesContext {
        var type = typeof(TContext);
        var compileMethod = typeof(AdminContextGenerator).GetMethod(nameof(CompilePage));
        
        var properties = type.GetProperties();

        var context = Activator.CreateInstance<TContext>();
        
        foreach (var property in properties) {
            var propertyType = property.PropertyType.GenericTypeArguments[0];
            var pageGeneratorType = typeof(AdminPageGenerator<>).MakeGenericType(propertyType);
            var generatorInstance = Activator.CreateInstance(pageGeneratorType, [propertyType.Name]);

            var populatorMethod = typeof(AdminContextGenerator)
                .GetMethod(nameof(ApplyConfigurationFromAttributes))?
                .MakeGenericMethod(propertyType);
            populatorMethod?.Invoke(this, [generatorInstance, propertyType.GetCustomAttributes()]);
            
            _adminPages.Add(propertyType, generatorInstance);
        }
        
        context.OnModelCreating(this);
        
        foreach (var property in properties) {
            var modelType = property.PropertyType.GenericTypeArguments[0];
            var method = compileMethod?.MakeGenericMethod(modelType);
            property.SetValue(context, method?.Invoke(this, []));
        }

        return context;
    }

    public void ApplyConfigurationFromAttributes<TModel>(IAdminPageGenerator<TModel> generator, object[] attributes) {
        if (attributes.Any(a => a is AdminNameAttribute)) {
            var attribute = attributes.Single(a => a is AdminNameAttribute) as AdminNameAttribute;
            generator.Title(attribute?.Name);
        }
            
        if (attributes.Any(a => a is AdminDescriptionAttribute)) {
            var attribute = attributes.Single(a => a is AdminDescriptionAttribute) as AdminDescriptionAttribute;
            generator.Description(attribute?.Description);
        }

        if (attributes.Any(a => a is AdminPermissionsAttribute)) {
            var attribute = attributes.Single(a => a is AdminPermissionsAttribute) as AdminPermissionsAttribute;
            generator.CreatePermission(attribute?.Permissions.Create);
            generator.UpdatePermission(attribute?.Permissions.Update);
            generator.ViewPermission(attribute?.Permissions.View);
            generator.DeletePermission(attribute?.Permissions.Delete);
        }

        if (attributes.Any(a => a is AdminButtonConfigAttribute)) {
            var attribute = attributes.Single(a => a is AdminButtonConfigAttribute) as AdminButtonConfigAttribute;
            generator.ShowCreateButton(attribute?.ShowCreateButton == true);
            generator.ShowUpdateButton(attribute?.ShowUpdateButton == true);
            generator.ShowDeleteButton(attribute?.ShowDeleteButton == true);
        }
    }

}