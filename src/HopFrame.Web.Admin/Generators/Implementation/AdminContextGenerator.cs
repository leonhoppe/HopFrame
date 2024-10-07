using HopFrame.Web.Admin.Models;
using HopFrame.Web.Admin.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal class AdminContextGenerator : IAdminContextGenerator {

    private readonly IDictionary<Type, object> _adminPages = new Dictionary<Type, object>();

    public IAdminPageGenerator<TModel> Page<TModel>() {
        if (_adminPages.TryGetValue(typeof(TModel), out var pageGenerator))
            return pageGenerator as IAdminPageGenerator<TModel>;

        var generator = Activator.CreateInstance(typeof(IAdminPageGenerator<TModel>)) as AdminPageGenerator<TModel>;
        generator?.ApplyConfigurationFromAttributes(typeof(TModel).GetCustomAttributes(false));
        
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
            var generatorInstance = Activator.CreateInstance(pageGeneratorType, [property.Name]); // Calls constructor with title attribute

            var populatorMethod = pageGeneratorType.GetMethod(nameof(AdminPageGenerator<TContext>.ApplyConfigurationFromAttributes));
            populatorMethod?.Invoke(generatorInstance, [propertyType.GetCustomAttributes(false)]);
            
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



    public static void RegisterPages(AdminPagesContext context, IAdminPagesProvider provider, IServiceCollection services) {
        var properties = context.GetType().GetProperties();

        foreach (var property in properties) {
            var page = property.GetValue(context) as AdminPage;
            if (page is null) continue;
            
            provider.RegisterAdminPage(page.Title.ToLower(), page);

            if (page.RepositoryProvider is not null)
                services.AddScoped(page.RepositoryProvider);
        }
    }

}