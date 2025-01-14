using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddHopFrameServices(this IServiceCollection services) {
        services.AddTransient<IContextExplorer, ContextExplorer>();
        return services;
    }
    
}