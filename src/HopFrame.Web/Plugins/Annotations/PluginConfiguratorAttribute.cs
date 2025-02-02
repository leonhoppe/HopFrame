namespace HopFrame.Web.Plugins.Annotations;

/// <summary>
/// Configures the method as a plugin configurator, so the method gets called, when the plugin is registered.
/// Only works on static methods
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class PluginConfiguratorAttribute : Attribute;
