namespace HopFrame.Core.Callbacks;

public interface ICallbackEmitter {
    
    Guid RegisterCallbackHandler(string @event, Func<object, IServiceProvider, Task> handler);
    
    bool RemoveCallbackHandler(Guid id);
    
    Task DispatchCallback(string @event, object argument = null!);

    void RemoveAllCallbackHandlers(string @event);
    void RemoveAllCallbackHandlers();

}