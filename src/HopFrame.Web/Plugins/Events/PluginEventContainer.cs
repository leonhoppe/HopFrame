using System.Reflection;

namespace HopFrame.Web.Plugins.Events;

internal sealed class PluginEventContainer {
    public required MethodInfo Handler { get; init; }
    public required Type EventType { get; init; }
    public required bool IsAwaitable { get; init; }
}