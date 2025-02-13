# Custom Views

You can also add your own pages to the HopFrame UI by defining the routes as custom views.
You can do that by using the following extension methods for the [](HopFrameConfig.md):

## Configuration methods

### AddCustomView (With configurator delegate)

Creates an entry to the side menu and dashboard with a custom URL.

```c#
HopFrameConfigurator AddCustomView(string name, string url, Action<CustomViewConfigurator> configuratorDelegate)
```

- **Parameters:**
    - `configurator`: The configurator for the HopFrame config that is being created.
    - `name`: The name of the navigation entry.
    - `url`: The target URL of the navigation entry.
    - `configuratorDelegate`: The delegate for configuring the view.

- **Returns:** `HopFrameConfigurator`

### AddCustomView (Without configurator delegate)

Creates an entry to the side menu and dashboard with a custom URL.

```c#
CustomViewConfigurator AddCustomView(string name, string url)
```

- **Parameters:**
    - `configurator`: The configurator for the HopFrame config that is being created.
    - `name`: The name of the navigation entry.
    - `url`: The target URL of the navigation entry.

- **Returns:** `CustomViewConfigurator`

## CustomViewConfigurator

### SetDescription

Sets the description displayed in the dashboard.

```c#
CustomViewConfigurator SetDescription(string description)
```

- **Parameters:**
    - `description`: The desired description.

- **Returns:** `CustomViewConfigurator`

### SetPolicy

Sets the policy needed in order to access the view.

```c#
CustomViewConfigurator SetPolicy(string policy)
```

- **Parameters:**
    - `policy`: The desired policy.

- **Returns:** `CustomViewConfigurator`

### SetIcon

Sets the icon displayed in the sidebar.

```c#
CustomViewConfigurator SetIcon(string icon)
```

- **Parameters:**
    - `icon`: The desired [fluent-icon](https://www.fluentui-blazor.net/Icon#explorer).

- **Returns:** `CustomViewConfigurator`

### SetLinkMatch

Sets the rule for the sidebar to determine if the link is active.

```c#
CustomViewConfigurator SetLinkMatch(NavLinkMatch match)
```

- **Parameters:**
    - `match`: The desired match rule.

- **Returns:** `CustomViewConfigurator`

## Example

```C#
builder.Services.AddHopFrame(options => {
    options.AddCustomView("Counter", "/counter")
        .SetDescription("A custom view")
        .SetPolicy("counter.view");
});
```
