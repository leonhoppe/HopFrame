namespace HopFrame.Web.Plugins.Annotations;

/// <summary>
/// Indicates, that the method is a handler for an event
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class EventHandlerAttribute : Attribute;
