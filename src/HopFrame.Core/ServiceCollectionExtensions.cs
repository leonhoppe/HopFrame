using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core;

/** An extension class to provide access to the setup of the library */
public static class ServiceCollectionExtensions {

    /** Configures the library using the provided configurator */
    public static void AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator) {
        var config = new HopFrameConfig();
        services.AddSingleton(config);

        services.AddTransient<IConfigAccessor, ConfigAccessor>();
        
        configurator.Invoke(new HopFrameConfigurator(config, services));
    }
    
}