using HopFrame.Core.Config;
using HopFrame.Core.Events;

namespace HopFrame.Core.Services.Implementations;

internal sealed class EventEmitter(IServiceProvider provider, HopFrameConfig config) : IEventEmitter {
    
    public Guid RegisterEventHandler(string @event, Func<object, IServiceProvider, Task> handler) {
        var handlerStore = new HopEventHandler(@event, handler);
        config.Handlers.Add(handlerStore);
        return handlerStore.Id;
    }
    
    public bool RemoveEventHandler(Guid id) {
        var count = config.Handlers.RemoveAll(handler => handler.Id == id);
        return count > 0;
    }
    
    public async Task DispatchEvent(string @event, object argument = null!) {
        var handlers = config.Handlers.Where(handler => handler.EventType == @event);
        var tasks = new List<Task>();
        
        foreach (var handler in handlers) {
            var task = handler.Handler.Invoke(argument, provider);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
    
    public void RemoveAllEventHandlers(string @event) {
        config.Handlers.RemoveAll(handler => handler.EventType == @event);
    }
    
    public void RemoveAllEventHandlers() {
        config.Handlers.Clear();
    }
    
}