using HopFrame.Web.Admin.Generators.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web.Admin;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddAdminContext<TContext>(this IServiceCollection services) where TContext : AdminPagesContext {
        services.AddSingleton(_ => {
            var generator = new AdminContextGenerator();
            return generator.CompileContext<TContext>();
        });

        return services;
    }
    
}