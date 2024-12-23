using HopFrame.Api.Controller;
using HopFrame.Api.Logic;
using HopFrame.Api.Logic.Implementation;
using HopFrame.Api.Models;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using HopFrame.Security.Authentication.OpenID;
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
    /// <param name="config">Configuration for how the HopFrame services get set up</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrame<TDbContext>(this IServiceCollection services, ConfigurationManager configuration, HopFrameApiModuleConfig config = null) where TDbContext : HopDbContextBase {
        config ??= new();
        
        var controllers = new List<Type>();
        
        if (config.ExposeModelEndpoints)
            controllers.AddRange([typeof(UserController), typeof(GroupController)]);
        
        var defaultAuthenticationSection = configuration.GetSection("HopFrame:Authentication:DefaultAuthentication");
        if (!defaultAuthenticationSection.Exists() || configuration.GetValue<bool>("HopFrame:Authentication:DefaultAuthentication"))
            controllers.Add(typeof(AuthController));
        
        if (configuration.GetValue<bool>("HopFrame:Authentication:OpenID:Enabled")) {
            IOpenIdAccessor.DefaultCallback = OpenIdController.DefaultCallback;
            controllers.Add(typeof(OpenIdController));
        }
        
        AddHopFrameNoEndpoints<TDbContext>(services, configuration, config);
        services.AddMvcCore().UseSpecificControllers(controllers.ToArray());
    }

    /// <summary>
    /// Adds all HopFrame services to the application
    /// </summary>
    /// <param name="services">The service provider to add the services to</param>
    /// <param name="configuration">The configuration used to configure HopFrame authentication</param>
    /// <param name="config">Configuration for how the HopFrame services get set up</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrameNoEndpoints<TDbContext>(this IServiceCollection services, ConfigurationManager configuration, HopFrameApiModuleConfig config = null) where TDbContext : HopDbContextBase {
        config ??= new();
        
        services.AddMvcCore().ConfigureApplicationPartManager(manager => {
            var endpoints = manager.ApplicationParts.SingleOrDefault(p => p.Name == typeof(ServiceCollectionExtensions).Namespace!.Replace(".Extensions", ""));
            manager.ApplicationParts.Remove(endpoints);
        });

        services.AddSingleton(config);
        services.AddHopFrameRepositories<TDbContext>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthLogic, AuthLogic>();
        services.AddScoped<IUserLogic, UserLogic>();
        services.AddScoped<IGroupLogic, GroupLogic>();
        
        services.AddHopFrameAuthentication(configuration, config);
    }
    
}
