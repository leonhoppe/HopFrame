using System.Reflection;
using HopFrame.Core.Config;
using HopFrame.Web.Components.Layout;
using HopFrame.Web.Models;
using HopFrame.Web.Plugins;
using HopFrame.Web.Plugins.Annotations;
using HopFrame.Web.Plugins.Internal;

namespace HopFrame.Web;

public static class HopFrameConfiguratorExtensions {
    
    /// <summary>
    /// Creates an entry to the side menu and dashboard with a custom url
    /// </summary>
    /// <param name="configurator">The configurator for the HopFrame config that is being created</param>
    /// <param name="name">The name of the navigation entry</param>
    /// <param name="url">The target url of the navigation entry</param>
    public static CustomViewConfigurator AddCustomView(this HopFrameConfigurator configurator, string name, string url) {
        var view = new CustomView {
            Name = name,
            Url = url
        };
        HopFrameLayout.CustomViews.Add(view);
        return new CustomViewConfigurator(view);
    }

    /// <param name="configuratorDelegate">The delegate for configuring the view</param>
    /// <inheritdoc cref="AddCustomView(HopFrame.Core.Config.HopFrameConfigurator,string,string)"/>
    public static HopFrameConfigurator AddCustomView(this HopFrameConfigurator configurator, string name, string url, Action<CustomViewConfigurator> configuratorDelegate) {
        var viewConfigurator = AddCustomView(configurator, name, url);
        configuratorDelegate.Invoke(viewConfigurator);
        return configurator;
    }

    /// <summary>
    /// Registers a plugin
    /// </summary>
    /// <param name="configurator">The configurator for the HopFrame config that is being created</param>
    /// <typeparam name="TPlugin">The plugin that should be registered</typeparam>
    public static HopFrameConfigurator AddPlugin<TPlugin>(this HopFrameConfigurator configurator) where TPlugin : HopFramePlugin {
        PluginOrchestrator.RegisterPlugin(configurator.ServiceCollection, typeof(TPlugin));

        var methods = typeof(TPlugin).GetMethods()
            .Where(method => method.IsStatic)
            .Where(method => method.GetCustomAttributes(true)
                .Any(attr => attr is PluginConfiguratorAttribute))
            .Where(method => method.GetParameters().Length < 2);
        
        foreach (var method in methods) {
            if (method.GetParameters().Length > 0)
                method.Invoke(null, [configurator]);
            else method.Invoke(null, []);
        }
            
        return configurator;
    }
    
}