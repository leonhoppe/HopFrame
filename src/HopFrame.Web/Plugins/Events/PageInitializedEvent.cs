using HopFrame.Core.Config;
using HopFrame.Web.Components.Pages;
using Microsoft.FluentUI.AspNetCore.Components;

namespace HopFrame.Web.Plugins.Events;

public class PageInitializedEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public List<PluginButton> PluginButtons { get; } = new();
    
    public void AddPageButton(string title, Func<Task> callback, bool pushRight = false, IconInfo? icon = null) {
        PluginButtons.Add(new() {
            Title = title,
            Icon = icon,
            Position = pushRight ? PluginButtonPosition.TopRight : PluginButtonPosition.TopLeft,
            Handler = (_, _) => callback.Invoke()
        });
    }

    public void AddPageButton(string title, Action callback, bool pushRight = false, IconInfo? icon = null) {
        AddPageButton(title, () => {
            callback.Invoke();
            return Task.CompletedTask;
        }, pushRight, icon);
    }

    public void AddEntityButton(IconInfo icon, Func<object, TableConfig, Task> callback) {
        PluginButtons.Add(new() {
            Icon = icon,
            Position = PluginButtonPosition.OnEntry,
            Handler = callback
        });
    }

    public void AddEntityButton(IconInfo icon, Action<object, TableConfig> callback) {
        AddEntityButton(icon, (obj, cfg) => {
            callback.Invoke(obj, cfg);
            return Task.CompletedTask;
        });
    }

    public void AddEntityButton<TEntity>(IconInfo icon, Func<TEntity, TableConfig, Task> callback) {
        PluginButtons.Add(new() {
            Icon = icon,
            Position = PluginButtonPosition.OnEntry,
            Handler = (obj, cfg) => callback.Invoke((TEntity)obj, cfg),
            TableFilter = typeof(TEntity)
        });
    }

    public void AddEntityButton<TEntity>(IconInfo icon, Action<TEntity, TableConfig> callback) {
        AddEntityButton<TEntity>(icon, (obj, cfg) => {
            callback.Invoke(obj, cfg);
            return Task.CompletedTask;
        });
    }

    public void AddPageButton<TEntity>(string title, Func<Task> callback, bool pushRight = false, IconInfo? icon = null) {
        PluginButtons.Add(new() {
            Title = title,
            Icon = icon,
            Position = pushRight ? PluginButtonPosition.TopRight : PluginButtonPosition.TopLeft,
            Handler = (_, _) => callback.Invoke(),
            TableFilter = typeof(TEntity)
        });
    }

    public void AddPageButton<TEntity>(string title, Action callback, bool pushRight = false, IconInfo? icon = null) {
        AddPageButton<TEntity>(title, () => {
            callback.Invoke();
            return Task.CompletedTask;
        }, pushRight, icon);
    }
}

public struct PluginButton {
    public PluginButtonPosition Position { get; set; }
    public Func<object, TableConfig, Task> Handler { get; set; }
    public string? Title { get; set; }
    public IconInfo? Icon { get; set; }
    public Type? TableFilter { get; set; }

    internal bool IsForTable(TableConfig? config) {
        if (config is null) return false;
        if (TableFilter is null) return true;
        return config.TableType == TableFilter;
    }
}

public enum PluginButtonPosition {
    TopLeft = 0,
    TopRight = 1,
    OnEntry = 2
}
