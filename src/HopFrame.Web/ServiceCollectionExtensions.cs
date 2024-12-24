using BlazorStrap;
using CurrieTechnologies.Razor.SweetAlert2;
using HopFrame.Database;
using HopFrame.Security.Authentication;
using HopFrame.Web.Admin;
using HopFrame.Web.Models;
using HopFrame.Web.Services;
using HopFrame.Web.Services.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddHopFrame<TDbContext>(this IServiceCollection services, ConfigurationManager configuration, HopFrameWebModuleConfig config = null) where TDbContext : HopDbContextBase {
        config ??= new HopFrameWebModuleConfig();
        services.AddHopFrameRepositories<TDbContext>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddTransient<AuthMiddleware>();
        services.AddAdminContext<HopAdminContext>();
        services.AddSingleton(config);
        
        // Component library's
        services.AddSweetAlert2();
        services.AddBlazorStrap();

        services.AddHopFrameAuthentication(configuration, config);

        return services;
    }

    public static RazorComponentsEndpointConventionBuilder AddHopFrameAdminPages(this RazorComponentsEndpointConventionBuilder builder) {
        return builder
            .DisableAntiforgery()
            .AddAdditionalAssemblies(typeof(ServiceCollectionExtensions).Assembly)
            .AddInteractiveServerRenderMode();
    }
}