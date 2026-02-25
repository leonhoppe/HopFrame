using HopFrame.Core;
using HopFrame.Core.Configurators;
using HopFrame.Web.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace HopFrame.Web;

/// An extension class to provide access to the setup of the library
public static class ServiceCollectionExtensions {
    
    /// Configures the library using the provided configurator
    public static IServiceCollection AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator) {
        services.AddHopFrameServices(configurator);

        services.AddMudServices();
        return services;
    }
    
    /// <summary>
    /// Adds the HopFrame admin ui endpoints
    /// </summary>
    public static RazorComponentsEndpointConventionBuilder AddHopFrame(this RazorComponentsEndpointConventionBuilder builder) {
        builder
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(ServiceCollectionExtensions).Assembly);

        return builder;
    }

    /// <summary>
    /// Adds the HopFrame admin ui endpoints
    /// </summary>
    public static WebApplication MapHopFrame(this WebApplication app) {
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        return app;
    }
    
}