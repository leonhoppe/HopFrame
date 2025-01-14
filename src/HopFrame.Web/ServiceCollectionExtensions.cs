using HopFrame.Core;
using HopFrame.Core.Config;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator) {
        var config = new HopFrameConfig();
        configurator.Invoke(new HopFrameConfigurator(config));

        services.AddSingleton(config);
        services.AddHopFrameServices();
        
        return services;
    }
    
}