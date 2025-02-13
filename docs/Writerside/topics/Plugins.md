# Plugins

If the default functionality of the HopFrame does not fit your needs, you can easily extend the pages
by using Plugins. They are registered as scoped services so you can use DI like everywhere else.

## Add a plugin

Create a class that extends the `HopFramePlugin` class:

```C#
public class SearchExtension : HopFramePlugin {
    
}
```

Then add the plugin to the HopFrame by using the extension method on the [](HopFrameConfig.md):

```C#
builder.Services.AddHopFrame(options => {
    options.AddPlugin<SearchExtension>();
});
```

## Configuring the plugin

If you want to change the HopFrame configuration from within your plugin, you can create a static method
and decorate it with the `PluginConfigurator` attribute. Here you can inject the `HopFrameConfigurator`
as an argument and change the configuration. Keep in mind, that this function automatically gets called
when you register your plugin, so any changes after that override the changes made in the plugin.

### Example

```C#
[PluginConfigurator]
public static void Configure(HopFrameConfigurator configurator) {
    configurator.AddCustomView("Counter", "/counter")
        .SetDescription("A custom view")
        .SetPolicy("counter.view");
}
```

## Events

The HopFrame provides various [events](Events.md) that can change how the corresponding action behaves. You can register
event handlers similar to the [configurator method](#configuring-the-plugin). Create a method, that is **not** static
and decorate it with the `EventHandler` attribute. This method can return either `void` or a `Task`. Then declare the
Event type as an argument and the function gets automatically registered as an event handler for the corresponding event.

### Examples

```C#
[EventHandler]
public async Task OnSearch(SearchEvent e) {
    var result = await searchHandler.Search(e.Table, e.SearchTerm);
    e.SetSearchResult(result.Items, result.TotalPages);
}

[EventHandler]
public void OnDelete(DeleteEntryEvent e) {
    cacheHandler.ClearCache(e.Entity);
}
```
