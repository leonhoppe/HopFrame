using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Core;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddHopFrameServices(this IServiceCollection services) {
        services.AddTransient<IContextExplorer, ContextExplorer>();
        services.TryAddTransient<IHopFrameAuthHandler, DefaultAuthHandler>();
        return services;
    }
    
}