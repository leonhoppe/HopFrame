using HopFrame.Web.Plugins.Events;

namespace HopFrame.Web.Plugins;

public interface IPluginOrchestrator {
    public Task<TEvent> DispatchEvent<TEvent>(TEvent @event, CancellationToken ct = new()) where TEvent : HopFrameEventArgs;
}