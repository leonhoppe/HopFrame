using HopFrame.Api.Controller;
using HopFrame.Api.Logic;
using HopFrame.Api.Logic.Implementation;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Api.Extensions;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Adds all HopFrame endpoints and services to the application
    /// </summary>
    /// <param name="services">The service provider to add the services to</param>
    /// <param name="configuration">The configuration used to configure HopFrame authentication</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrame<TDbContext>(this IServiceCollection services, ConfigurationManager configuration) where TDbContext : HopDbContextBase {
        var controllers = new List<Type> { typeof(AuthController) };
        
        if (configuration.GetValue<bool>("HopFrame:Authentication:OpenID:Enabled"))
            controllers.Add(typeof(OpenIdController));
        
        AddHopFrameNoEndpoints<TDbContext>(services, configuration);
        services.AddMvcCore().UseSpecificControllers(controllers.ToArray());
    }
    
    /// <summary>
    /// Adds all HopFrame services to the application
    /// </summary>
    /// <param name="services">The service provider to add the services to</param>
    /// <param name="configuration">The configuration used to configure HopFrame authentication</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrameNoEndpoints<TDbContext>(this IServiceCollection services, ConfigurationManager configuration) where TDbContext : HopDbContextBase {
        services.AddMvcCore().ConfigureApplicationPartManager(manager => {
            var endpoints = manager.ApplicationParts.SingleOrDefault(p => p.Name == typeof(ServiceCollectionExtensions).Namespace!.Replace(".Extensions", ""));
            manager.ApplicationParts.Remove(endpoints);
        });
        
        services.AddHopFrameRepositories<TDbContext>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthLogic, AuthLogic>();
        
        services.AddHopFrameAuthentication(configuration);
    }
    
}
