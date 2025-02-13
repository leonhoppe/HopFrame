# HopFrameConfig

The HopFrame config is the global object containing all configurations made for the HopFrame.
It is registered as a singleton and can be injected by any service.
But it should be treated as a read only dependency because changing the configuration during runtime is not tested and may cause bugs.

## Changing the configuration

As already mentioned in the [](Installation.md), you configure the HopFrame using the extension method of the `IServiceCollection` named `AddHopFrame`.
This extension method eiter takes a `HopFrameConfig` or a configurator action with a `HopFrameConfigurator` as an argument.
You can optionally also provide a `LibraryConfiguration` for the Fluent UI library and a toggle named `addRazorComponents` which disables the calls for adding
Razor pages with interactive server components if you want to do this yourself.

### Mapping the HopFrame pages

In order for the HopFrame pages to be served you need to add them to your application.
You can do that in two ways:

1. Just use the HopFrame as the razor host (Useful for APIs)
    Just map the HopFrame before you run your application:
    ```c#
    app.MapHopFrame();
    ```

2. Add the HopFrame to your Razor container (Useful for Blazor web apps)
    Add the HopFrame to the `MapRazorComponents` call:
    ```C#
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddHopFramePages();
    ```

### Example

```C#
builder.Services.AddHopFrame(options => {
    options.DisplayUserInfo(false);
    options.AddDbContext<DatabaseContext>(context => {
        context.Table<User>(table => {
            table.Property(u => u.Password)
                .DisplayValue(false);

            table.Property(u => u.Id)
                .IsSortable(false)
                .SetOrderIndex(3);

            table.SetViewPolicy("policy");

            table.Property(u => u.Posts)
                .FormatEach<Post>((post, _) => post.Caption);
        });
    });

    options.AddCustomView("Counter", "/counter")
        .SetDescription("A custom view")
        .SetPolicy("counter.view");
});
```

## Configuration methods

### AddDbContext (With configurator)

Adds all tables defined in the `DbContext` to the HopFrame UI and configures it using the provided configurator.

```c#
HopFrameConfigurator AddDbContext<TDbContext>(Action<DbContextConfigurator<TDbContext>> configurator) where TDbContext : DbContext
```

- **Type Parameters:**
    - `TDbContext`: The `DbContext` from which all tables should be added.

- **Parameters:**
    - `configurator`: Used for configuring the `DbContext`.

- **Returns:** `HopFrameConfigurator`

- **See Also:** [](DbContextConfig.md)

### AddDbContext (without configurator)

Adds all tables defined in the `DbContext` to the HopFrame UI and configures it.

```c#
DbContextConfigurator<TDbContext> AddDbContext<TDbContext>() where TDbContext : DbContext
```

- **Type Parameters:**
    - `TDbContext`: The `DbContext` from which all tables should be added.

- **Returns:** `DbContextConfigurator<TDbContext>`

- **See Also:** [](DbContextConfig.md)

### HasDbContext

Checks if a context is already registered in the HopFrame.

```c#
bool HasDbContext<TDbContext>() where TDbContext : DbContext
```

- **Type Parameters:**
    - `TDbContext`: The context that should be checked.

- **Returns:** `true` if the context is already registered, `false` if not.

### GetDbContext

Returns a configurator for the context if it was already defined.

```c#
DbContextConfigurator<TDbContext>? GetDbContext<TDbContext>() where TDbContext : DbContext
```

- **Type Parameters:**
    - `TDbContext`

- **Returns:** The configurator of the context if it already was defined, `null` if not.

### DisplayUserInfo

Determines if the name of the currently logged-in user should be displayed in the top right corner of the admin UI.

```c#
HopFrameConfigurator DisplayUserInfo(bool display)
```

- **Parameters:**
    - `display`: A boolean value to set if the user info should be displayed.

- **Returns:** `HopFrameConfigurator`

### SetBasePolicy

Sets a default policy that every user needs to have in order to access the admin UI.

```c#
HopFrameConfigurator SetBasePolicy(string basePolicy)
```

- **Parameters:**
    - `basePolicy`: The default policy string.

- **Returns:** `HopFrameConfigurator`

### SetLoginPage

Sets a custom login page to redirect to if the request to the admin UI was unauthorized.

```c#
HopFrameConfigurator SetLoginPage(string url)
```

- **Parameters:**
    - `url`: The URL of the custom login page.

- **Returns:** `HopFrameConfigurator`

