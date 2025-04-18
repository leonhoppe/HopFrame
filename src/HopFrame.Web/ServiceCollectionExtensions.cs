using HopFrame.Core;
using HopFrame.Core.Config;
using HopFrame.Core.Callbacks;
using HopFrame.Web.Components;
using HopFrame.Web.Components.Pages;
using HopFrame.Web.Plugins;
using HopFrame.Web.Plugins.Internal;
using HopFrame.Web.Services;
using HopFrame.Web.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Configures the HopFrame using the provided configurator and adds all internal HopFrame services including the default insecure auth handler if not already provided
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="configurator">The configurator used to build the HopFrame configuration</param>
    /// <param name="fluentUiLibraryConfiguration">The configuration for the FluentUI components</param>
    /// <param name="addRazorComponents">Set this to false if you don't want to automatically configure razor components with interactive server components</param>
    /// <returns>The same service collection that is passed in</returns>
    public static IServiceCollection AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator, LibraryConfiguration? fluentUiLibraryConfiguration = null, bool addRazorComponents = true) {
        var config = new HopFrameConfig();
        configurator.Invoke(new HopFrameConfigurator(config, services));
        return AddHopFrame(services, config, fluentUiLibraryConfiguration, addRazorComponents);
    }

    /// <summary>
    /// Configures the HopFrame using the provided configurator and adds all internal HopFrame services including the default insecure auth handler if not already provided
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="config">The config used for the HopFrame admin ui</param>
    /// <param name="fluentUiLibraryConfiguration">The configuration for the FluentUI components</param>
    /// <param name="addRazorComponents">Set this to false if you don't want to automatically configure razor components with interactive server components</param>
    /// <returns>The same service collection that is passed in</returns>
    public static IServiceCollection AddHopFrame(this IServiceCollection services, HopFrameConfig config, LibraryConfiguration? fluentUiLibraryConfiguration = null, bool addRazorComponents = true) {
        services.AddSingleton(config);
        services.AddHopFrameServices();
        services.AddFluentUIComponents(fluentUiLibraryConfiguration);

        services.AddScoped<IPluginOrchestrator, PluginOrchestrator>();
        services.AddScoped<IFileService, FileService>();
        services.TryAddScoped<ISearchSuggestionProvider, SearchSuggestionProvider>();

        if (addRazorComponents) {
            services.AddRazorComponents()
                .AddInteractiveServerComponents();
        }
        
        return services;
    }

    /// <summary>
    /// Adds the HopFrame admin ui endpoints
    /// </summary>
    /// <seealso cref="AddHopFramePages"/>
    [Obsolete($"Use '{nameof(AddHopFramePages)}' instead")]
    public static RazorComponentsEndpointConventionBuilder MapHopFramePages(this RazorComponentsEndpointConventionBuilder builder) {
        return AddHopFramePages(builder);
    }
    
    /// <summary>
    /// Adds the HopFrame admin ui endpoints
    /// </summary>
    public static RazorComponentsEndpointConventionBuilder AddHopFramePages(this RazorComponentsEndpointConventionBuilder builder) {
        builder
            .AddInteractiveServerRenderMode()
            .AddAdditionalAssemblies(typeof(HopFrameHome).Assembly);
        return builder;
    }

    public static WebApplication MapHopFrame(this WebApplication app) {
        app.UseAntiforgery();
        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();
        return app;
    }
    
}