using HopFrame.Core;
using HopFrame.Core.Config;
using HopFrame.Web.Components.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Builder;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Configures the HopFrame using the provided configurator and adds all internal HopFrame services including the default insecure auth handler if not already provided
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="configurator">The configurator used to build the HopFrame configuration</param>
    /// <param name="fluentUiLibraryConfiguration">The configuration for the FluentUI components</param>
    /// <returns>The same service collection that is passed in</returns>
    public static IServiceCollection AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator, LibraryConfiguration? fluentUiLibraryConfiguration = null) {
        var config = new HopFrameConfig();
        configurator.Invoke(new HopFrameConfigurator(config));
        return AddHopFrame(services, config, fluentUiLibraryConfiguration);
    }

    /// <summary>
    /// Configures the HopFrame using the provided configurator and adds all internal HopFrame services including the default insecure auth handler if not already provided
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="config">The config used for the HopFrame admin ui</param>
    /// <param name="fluentUiLibraryConfiguration">The configuration for the FluentUI components</param>
    /// <returns>The same service collection that is passed in</returns>
    public static IServiceCollection AddHopFrame(this IServiceCollection services, HopFrameConfig config, LibraryConfiguration? fluentUiLibraryConfiguration = null) {
        services.AddSingleton(config);
        services.AddHopFrameServices();
        services.AddFluentUIComponents(fluentUiLibraryConfiguration);
        return services;
    }

    public static RazorComponentsEndpointConventionBuilder MapHopFramePages(this RazorComponentsEndpointConventionBuilder builder) {
        builder
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(HopFrameHome).Assembly);
        return builder;
    }
    
}