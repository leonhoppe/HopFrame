using HopFrame.Core.Config;
using HopFrame.Web.Components.Layout;
using HopFrame.Web.Models;

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
    
}