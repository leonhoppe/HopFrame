using HopFrame.Core.Config;
using HopFrame.Core.Callbacks;

namespace HopFrame.Core.Services.Implementations;

internal sealed class CallbackEmitter(IServiceProvider provider, HopFrameConfig config) : ICallbackEmitter {
    
    public Guid RegisterCallbackHandler(string @event, Func<object, IServiceProvider, Task> handler) {
        var handlerStore = new HopCallbackHandler(@event, handler);
        config.Handlers.Add(handlerStore);
        return handlerStore.Id;
    }
    
    public bool RemoveCallbackHandler(Guid id) {
        var count = config.Handlers.RemoveAll(handler => handler.Id == id);
        return count > 0;
    }
    
    public async Task DispatchCallback(string @event, object argument = null!) {
        var handlers = config.Handlers.Where(handler => handler.EventType == @event);
        var tasks = new List<Task>();
        
        foreach (var handler in handlers) {
            var task = handler.Handler.Invoke(argument, provider);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
    
    public void RemoveAllCallbackHandlers(string @event) {
        config.Handlers.RemoveAll(handler => handler.EventType == @event);
    }
    
    public void RemoveAllCallbackHandlers() {
        config.Handlers.Clear();
    }
    
}