using BlazorStrap;
using CurrieTechnologies.Razor.SweetAlert2;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using HopFrame.Web.Services;
using HopFrame.Web.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddHopFrame<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddHttpClient();
        services.AddHopFrameRepositories<TDbContext>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<AuthMiddleware>();
        
        // Component library's
        services.AddSweetAlert2();
        services.AddBlazorStrap();

        services.AddHopFrameAuthentication();

        return services;
    }

    public static RazorComponentsEndpointConventionBuilder AddHopFrameAdminPages(this RazorComponentsEndpointConventionBuilder builder) {
        return builder
            .DisableAntiforgery()
            .AddAdditionalAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddInteractiveServerRenderMode();
    }
}