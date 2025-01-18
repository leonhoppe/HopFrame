using HopFrame.Core;
using HopFrame.Core.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator, LibraryConfiguration? fluentUiLibraryConfiguration = null) {
        var config = new HopFrameConfig();
        configurator.Invoke(new HopFrameConfigurator(config));
        return AddHopFrame(services, config, fluentUiLibraryConfiguration);
    }

    public static IServiceCollection AddHopFrame(this IServiceCollection services, HopFrameConfig config, LibraryConfiguration? fluentUiLibraryConfiguration = null) {
        services.AddSingleton(config);
        services.AddHopFrameServices();
        services.AddFluentUIComponents(fluentUiLibraryConfiguration);
        return services;
    }
    
}