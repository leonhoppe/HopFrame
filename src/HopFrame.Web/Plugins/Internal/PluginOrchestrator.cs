using HopFrame.Web.Plugins.Annotations;
using HopFrame.Web.Plugins.Events;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Web.Plugins.Internal;

internal sealed class PluginOrchestrator(IServiceProvider services) : IPluginOrchestrator {

    public static void RegisterPlugin(IServiceCollection collection, Type plugin) {
        var methods = plugin.GetMethods()
            .Where(method => method.GetCustomAttributes(true)
                .Any(attr => attr is EventHandlerAttribute));

        foreach (var method in methods) {
            var awaitable = method.ReturnType.IsAssignableFrom(typeof(Task));
            var eventType = method
                .GetParameters()
                .FirstOrDefault(param => param.ParameterType.IsAssignableTo(typeof(HopFrameEventArgs)))?.ParameterType;
            
            if (eventType is null) continue;
            var container = new PluginEventContainer {
                EventType = eventType,
                IsAwaitable = awaitable,
                Handler = method
            };
            collection.AddSingleton(container);
            collection.AddScoped(plugin);
        }
    }

    public async Task<TEvent> DispatchEvent<TEvent>(TEvent @event, CancellationToken ct = new()) where TEvent : HopFrameEventArgs {
        var eventContainers = services.GetRequiredService<IEnumerable<PluginEventContainer>>()
            .Where(container => container.EventType == typeof(TEvent));

        var eventType = typeof(TEvent);
        var tokenType = typeof(CancellationToken);
        foreach (var container in eventContainers) {
            var plugin = services.GetRequiredService(container.Handler.DeclaringType!);
            var parameters = new List<object?>();

            foreach (var parameter in container.Handler.GetParameters()) {
                if (parameter.ParameterType == eventType)
                    parameters.Add(@event);
                else if (parameter.ParameterType == tokenType)
                    parameters.Add(ct);
                else parameters.Add(null);
            }
            
            var result = container.Handler.Invoke(plugin, parameters.ToArray());

            if (container.IsAwaitable)
                await (Task)result!;
        }

        return @event;
    }
    
}