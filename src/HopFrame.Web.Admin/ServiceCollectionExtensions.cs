using HopFrame.Web.Admin.Generators.Implementation;
using HopFrame.Web.Admin.Providers;
using HopFrame.Web.Admin.Providers.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Web.Admin;

public static class ServiceCollectionExtensions {

    private static IAdminPagesProvider _provider;

    public static IServiceCollection AddAdminContext<TContext>(this IServiceCollection services) where TContext : AdminPagesContext {
        var provider = GetProvider();
        services.TryAddSingleton(provider);
        
        var generator = new AdminContextGenerator();
        var context = generator.CompileContext<TContext>();
        AdminContextGenerator.RegisterPages(context, provider, services);
        services.AddSingleton(context);

        return services;
    }

    private static IAdminPagesProvider GetProvider() {
        return _provider ??= new AdminPagesProvider();
    }
    
}