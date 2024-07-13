using HopFrame.Api.Controller;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Api.Extensions;

public static class ServiceCollectionExtensions {

    public static void AddHopFrame<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddMvcCore().UseSpecificControllers(typeof(SecurityController<TDbContext>));
        services.AddHopFrameAuthentication<TDbContext>();
    }
    
}
