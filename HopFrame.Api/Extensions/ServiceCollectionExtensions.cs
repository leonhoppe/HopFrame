using HopFrame.Api.Controller;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Api.Extensions;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Adds all HopFrame endpoints and the HopFrame security layer to the WebApplication
    /// </summary>
    /// <param name="services">The service provider to add the services to</param>
    /// <typeparam name="TDbContext">The data source for all HopFrame entities</typeparam>
    public static void AddHopFrame<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddMvcCore().UseSpecificControllers(typeof(SecurityController<TDbContext>));
        services.AddHopFrameAuthentication<TDbContext>();
    }
    
}
