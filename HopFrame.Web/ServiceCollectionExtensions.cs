using CurrieTechnologies.Razor.SweetAlert2;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using HopFrame.Web.Services;
using HopFrame.Web.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddHopFrameServices<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddHttpClient();
        services.AddScoped<IAuthService, AuthService<TDbContext>>();
        services.AddTransient<AuthMiddleware>();
        
        // Component library's
        services.AddSweetAlert2();
        
        //TODO: Use https://blazorstrap.io/V5/V5

        services.AddHopFrameAuthentication<TDbContext>();

        return services;
    }

    public static RazorComponentsEndpointConventionBuilder AddHopFramePages(this RazorComponentsEndpointConventionBuilder builder) {
        return builder
            .AddAdditionalAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddInteractiveServerRenderMode()
            .DisableAntiforgery(); //TODO: Make Antiforgery work
    }
}