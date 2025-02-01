namespace HopFrame.Core.Events;

public readonly struct HopEventHandler(string eventType, Func<object, IServiceProvider, Task> handler) {
    public Guid Id { get; } = Guid.CreateVersion7();
    public Func<object, IServiceProvider, Task> Handler { get; } = handler;
    public string EventType { get; } = eventType;
}
