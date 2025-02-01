using HopFrame.Core.Events;
using HopFrame.Core.Services;
using HopFrame.Core.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Core;

public static class ServiceCollectionExtensions {

    /// <summary>
    /// Adds all internal HopFrame services used by the web ui including the default insecure auth handler if not already provided
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <returns>The same service collection that is passed in</returns>
    public static IServiceCollection AddHopFrameServices(this IServiceCollection services) {
        services.AddScoped<IContextExplorer, ContextExplorer>();
        services.TryAddScoped<IHopFrameAuthHandler, DefaultAuthHandler>();
        services.TryAddScoped<IEventEmitter, EventEmitter>();
        return services;
    }
    
}