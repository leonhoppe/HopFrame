using HopFrame.Core.Configurators;
using HopFrame.Web.Components;
using HopFrame.Web.Components.Pages;

namespace HopFrame.Web;

public static class HopFrameConfiguratorExtensions {
    
    /// <inheritdoc cref="CustomPage"/>
    public static HopFrameConfigurator AddCustomPage(this HopFrameConfigurator configurator, CustomPage page) {
        HomePage.CustomPages.Add(page);
        return configurator;
    }
    
}

/// <summary>
/// A custom entry in the sidebar and on the dashboard<br/>
/// Make sure to set the <see cref="HopFrameLayout"/> as the layout of the page
/// so it's integrated into the ui
/// </summary>
/// <param name="Name">The name of the entry</param>
/// <param name="Description">The description of the entry</param>
/// <param name="Icon">The icon of the entry</param>
/// <param name="Route">The href of the entry</param>
/// <param name="OrderIndex">The sort index of the entry</param>
/// <param name="AsIFrame">Integrates the page into the layout even when the page itself has a different layout</param>
public readonly record struct CustomPage(
    string Name,
    string? Description,
    string Icon,
    string Route,
    int OrderIndex,
    bool AsIFrame = false) {

    internal string GetPageRoute() {
        if (AsIFrame)
            return "/admin/page/" + Name;
        
        return Route;
    }
    
}
