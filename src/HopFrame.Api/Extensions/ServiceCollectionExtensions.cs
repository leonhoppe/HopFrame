using HopFrame.Api.Controller;
using HopFrame.Api.Logic;
using HopFrame.Api.Logic.Implementation;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Api.Extensions;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Adds all HopFrame endpoints and services to the application
    /// </summary>
    /// <param name="services">The service provider to add the services to</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrame<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddMvcCore().UseSpecificControllers(typeof(SecurityController));
        AddHopFrameNoEndpoints<TDbContext>(services);
    }
    
    /// <summary>
    /// Adds all HopFrame services to the application
    /// </summary>
    /// <param name="services">The service provider to add the services to</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrameNoEndpoints<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthLogic, AuthLogic<TDbContext>>();
        
        services.AddHopFrameAuthentication<TDbContext>();
    }
    
}
