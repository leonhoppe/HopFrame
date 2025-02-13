using Microsoft.AspNetCore.Components.Routing;
using Microsoft.FluentUI.AspNetCore.Components;

namespace HopFrame.Web.Models;

public sealed class CustomView {
    public required string Name { get; init; }
    public string? Description { get; set; }
    public string? Policy { get; set; }
    public required string Url { get; init; }
    public string Icon { get; set; } = "Window";
    public NavLinkMatch LinkMatch { get; set; } = NavLinkMatch.All;
}

public sealed class CustomViewConfigurator(CustomView view) {
    public CustomView InnerConfig { get; } = view;

    /// <summary>
    /// Sets the description displayed in the dashboard
    /// </summary>
    /// <param name="description">The desired description</param>
    public CustomViewConfigurator SetDescription(string description) {
        InnerConfig.Description = description;
        return this;
    }

    /// <summary>
    /// Sets the policy needed in order to access the view
    /// </summary>
    /// <param name="policy">The desired policy</param>
    public CustomViewConfigurator SetPolicy(string policy) {
        InnerConfig.Policy = policy;
        return this;
    }

    /// <summary>
    /// Sets the icon displayed in the sidebar
    /// </summary>
    /// <param name="icon">The desired <see href="https://www.fluentui-blazor.net/Icon#explorer">fluent-icon</see></param>
    public CustomViewConfigurator SetIcon(string icon) {
        InnerConfig.Icon = icon;
        return this;
    }

    /// <summary>
    /// Sets the rule for the sidebar to determine if the link is active
    /// </summary>
    /// <param name="match">The desired match rule</param>
    public CustomViewConfigurator SetLinkMatch(NavLinkMatch match) {
        InnerConfig.LinkMatch = match;
        return this;
    }
    
}
