using HopFrame.Core.Config;
using HopFrame.Web.Components.Pages;
using Microsoft.FluentUI.AspNetCore.Components;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// Raised when a table is initialized
/// </summary>
/// <param name="sender"></param>
public class TableInitializedEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    /// <summary>
    /// List of all buttons added by the plugins
    /// </summary>
    public List<PluginButton> PluginButtons { get; } = new();
    
    /// <summary>
    /// Toggles for the native buttons on the page
    /// </summary>
    public DefaultButtonToggles DefaultButtons { get; set; } = new();
    
    /// <summary>
    /// Adds a custom button to the top bar of the page
    /// </summary>
    /// <param name="title">The text displayed on the button</param>
    /// <param name="callback">The function that is invoked when the button is pressed</param>
    /// <param name="pushRight">Determines if the button should be displayed on the right</param>
    /// <param name="icon">Optional: The icon displayed next to the title</param>
    public void AddPageButton(string title, Func<Task> callback, bool pushRight = false, IconInfo? icon = null) {
        PluginButtons.Add(new() {
            Title = title,
            Icon = icon,
            Position = pushRight ? PluginButtonPosition.TopRight : PluginButtonPosition.TopLeft,
            Handler = (_, _) => callback.Invoke()
        });
    }
    
    /// <inheritdoc cref="AddPageButton(string,System.Func{System.Threading.Tasks.Task},bool,Microsoft.FluentUI.AspNetCore.Components.IconInfo?)"/>
    public void AddPageButton(string title, Action callback, bool pushRight = false, IconInfo? icon = null) {
        AddPageButton(title, () => {
            callback.Invoke();
            return Task.CompletedTask;
        }, pushRight, icon);
    }

    /// <summary>
    /// Adds a custom button to the Actions column next to every entry
    /// </summary>
    /// <param name="icon">The icon displayed in the button</param>
    /// <param name="callback">The function that is invoked when the button is pressed</param>
    public void AddEntityButton(IconInfo icon, Func<object, TableConfig, Task> callback) {
        PluginButtons.Add(new() {
            Icon = icon,
            Position = PluginButtonPosition.OnEntry,
            Handler = callback
        });
    }

    /// <inheritdoc cref="AddEntityButton(IconInfo,System.Func{object,TableConfig,System.Threading.Tasks.Task})"/>
    public void AddEntityButton(IconInfo icon, Action<object, TableConfig> callback) {
        AddEntityButton(icon, (obj, cfg) => {
            callback.Invoke(obj, cfg);
            return Task.CompletedTask;
        });
    }

    /// <typeparam name="TEntity">The entity type of the table that should display this button</typeparam>
    /// <inheritdoc cref="AddEntityButton(IconInfo,System.Func{object,TableConfig,System.Threading.Tasks.Task})"/>
    public void AddEntityButton<TEntity>(IconInfo icon, Func<TEntity, TableConfig, Task> callback) {
        PluginButtons.Add(new() {
            Icon = icon,
            Position = PluginButtonPosition.OnEntry,
            Handler = (obj, cfg) => callback.Invoke((TEntity)obj, cfg),
            TableFilter = typeof(TEntity)
        });
    }

    /// <typeparam name="TEntity">The entity type of the table that should display this button</typeparam>
    /// <inheritdoc cref="AddEntityButton(IconInfo,System.Func{object,TableConfig,System.Threading.Tasks.Task})"/>
    public void AddEntityButton<TEntity>(IconInfo icon, Action<TEntity, TableConfig> callback) {
        AddEntityButton<TEntity>(icon, (obj, cfg) => {
            callback.Invoke(obj, cfg);
            return Task.CompletedTask;
        });
    }

    /// <typeparam name="TEntity">The entity type of the table that should display this button</typeparam>
    /// <inheritdoc cref="AddPageButton(string,System.Func{System.Threading.Tasks.Task},bool,Microsoft.FluentUI.AspNetCore.Components.IconInfo?)"/>
    public void AddPageButton<TEntity>(string title, Func<Task> callback, bool pushRight = false, IconInfo? icon = null) {
        PluginButtons.Add(new() {
            Title = title,
            Icon = icon,
            Position = pushRight ? PluginButtonPosition.TopRight : PluginButtonPosition.TopLeft,
            Handler = (_, _) => callback.Invoke(),
            TableFilter = typeof(TEntity)
        });
    }

    /// <typeparam name="TEntity">The entity type of the table that should display this button</typeparam>
    /// <inheritdoc cref="AddPageButton(string,System.Func{System.Threading.Tasks.Task},bool,Microsoft.FluentUI.AspNetCore.Components.IconInfo?)"/>
    public void AddPageButton<TEntity>(string title, Action callback, bool pushRight = false, IconInfo? icon = null) {
        AddPageButton<TEntity>(title, () => {
            callback.Invoke();
            return Task.CompletedTask;
        }, pushRight, icon);
    }
}

/// <summary>
/// A wrapper object containing all information about a custom button
/// </summary>
public struct PluginButton {
    /// <summary>
    /// The position of the button
    /// </summary>
    public PluginButtonPosition Position { get; set; }
    
    /// <summary>
    /// The function that is invoked when the button is pressed
    /// </summary>
    public Func<object, TableConfig, Task> Handler { get; set; }
    
    /// <summary>
    /// The text on the button if supported 
    /// </summary>
    public string? Title { get; set; }
    
    /// <summary>
    /// The icon displayed on the button
    /// </summary>
    public IconInfo? Icon { get; set; }
    
    /// <summary>
    /// The entity type of the table that should display this button
    /// </summary>
    public Type? TableFilter { get; set; }

    internal bool IsForTable(TableConfig? config) {
        if (config is null) return false;
        if (TableFilter is null) return true;
        return config.TableType == TableFilter;
    }
}

/// <summary>
/// All available positions for a custom button
/// </summary>
public enum PluginButtonPosition {
    TopLeft = 0,
    TopRight = 1,
    OnEntry = 2
}

/// <summary>
/// Toggles for the native buttons on the page
/// </summary>
public struct DefaultButtonToggles() {
    public bool ShowRefreshButton { get; set; } = true;
    public bool ShowAddEntityButton { get; set; } = true;
    public bool ShowDeleteButton { get; set; } = true;
    public bool ShowEditButton { get; set; } = true;
}
