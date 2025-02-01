namespace HopFrame.Core.Events;

public interface IEventEmitter {
    
    Guid RegisterEventHandler(string @event, Func<object, IServiceProvider, Task> handler);
    
    bool RemoveEventHandler(Guid id);
    
    Task DispatchEvent(string @event, object argument = null!);

    void RemoveAllEventHandlers(string @event);
    void RemoveAllEventHandlers();

}