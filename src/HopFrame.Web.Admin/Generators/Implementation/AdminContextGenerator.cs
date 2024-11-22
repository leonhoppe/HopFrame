using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Models;

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

    public TContext CompileContext<TContext>(IServiceProvider provider) where TContext : AdminPagesContext {
        var type = typeof(TContext);
        var compileMethod = typeof(AdminContextGenerator).GetMethod(nameof(CompilePage));
        
        var properties = type.GetProperties();

        var dependencies = ResolveDependencies<TContext>(provider);
        var context = Activator.CreateInstance(type, dependencies) as TContext;
        
        foreach (var property in properties) {
            var propertyType = property.PropertyType.GenericTypeArguments[0];
            var pageGeneratorType = typeof(AdminPageGenerator<>).MakeGenericType(propertyType);
            var generatorInstance = Activator.CreateInstance(pageGeneratorType);

            var titleMethod = pageGeneratorType.GetMethod(nameof(AdminPageGenerator<TContext>.Title));
            titleMethod?.Invoke(generatorInstance, [property.Name]);

            var populateMethod = pageGeneratorType.GetMethod(nameof(AdminPageGenerator<TContext>.ApplyConfigurationFromAttributes));
            populateMethod?.Invoke(generatorInstance, [propertyType.GetCustomAttributes(false)]);
            
            _adminPages.Add(propertyType, generatorInstance);
        }
        
        context?.OnModelCreating(this);
        
        foreach (var property in properties) {
            var modelType = property.PropertyType.GenericTypeArguments[0];
            var method = compileMethod?.MakeGenericMethod(modelType);
            var compiledPage = method?.Invoke(this, []) as AdminPage;
            
            var url = property.Name;
            if (property.GetCustomAttributes(false).Any(a => a is AdminPageUrlAttribute)) {
                var attribute = property.GetCustomAttributes(false)
                    .Single(a => a is AdminPageUrlAttribute) as AdminPageUrlAttribute;

                url = attribute?.Url;
            }
            compiledPage!.Url = url;
            
            property.SetValue(context, compiledPage);
        }

        return context;
    }

    private object[] ResolveDependencies<TContext>(IServiceProvider provider) {
        return ResolveDependencies(typeof(TContext), provider);
    }
    
    public static object[] ResolveDependencies(Type type, IServiceProvider provider) {
        var ctors = type.GetConstructors();

        if (ctors.Length == 0) return [];
        if (ctors.Length > 1)
            throw new ArgumentException($"Dependencies of {type.Name} could not be resolved (multiple constructors)!");

        var ctor = ctors[0];
        var depTypes = ctor.GetParameters();
        var dependencies = new object[depTypes.Length];
        
        for (var i = 0; i < depTypes.Length; i++) {
            dependencies[i] = provider.GetService(depTypes[i].ParameterType);
        }

        return dependencies;
    }

}