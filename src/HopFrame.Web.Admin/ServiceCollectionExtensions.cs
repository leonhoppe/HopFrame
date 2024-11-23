using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Generators.Implementation;
using HopFrame.Web.Admin.Providers;
using HopFrame.Web.Admin.Providers.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Web.Admin;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddAdminContext<TContext>(this IServiceCollection services) where TContext : AdminPagesContext {
        services.TryAddSingleton<IAdminPagesProvider, AdminPagesProvider>();
        
        services.AddSingleton(provider => {
            var generator = new AdminContextGenerator();
            var context = generator.CompileContext<TContext>(provider);
            return context;
        });
        
        PreregisterPages<TContext>();

        return services;
    }

    private static void PreregisterPages<TContext>() where TContext : AdminPagesContext {
        var contextType = typeof(TContext);
        var props = contextType.GetProperties();

        foreach (var property in props) {
            var url = property.Name;

            if (property.GetCustomAttributes(false).Any(a => a is AdminPageUrlAttribute)) {
                var attribute = property.GetCustomAttributes(false)
                    .Single(a => a is AdminPageUrlAttribute) as AdminPageUrlAttribute;

                url = attribute?.Url;
            }
            
            AdminPagesProvider.RegisterAdminPage<TContext>(url, property.PropertyType);
        }
    }
    
}