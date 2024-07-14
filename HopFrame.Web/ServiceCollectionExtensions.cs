using HopFrame.Database;
using HopFrame.Web.Services;
using HopFrame.Web.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddHopFrameServices<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddHttpClient();
        services.AddScoped<IAuthService, AuthService<TDbContext>>();

        return services;
    }

    public static RazorComponentsEndpointConventionBuilder AddHopFramePages(this RazorComponentsEndpointConventionBuilder builder) {
        return builder.AddAdditionalAssemblies(typeof(ServiceCollectionExtensions).Assembly);
    }
}