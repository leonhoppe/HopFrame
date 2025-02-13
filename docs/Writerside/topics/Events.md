# Events

## Base event

Every event inherits from the base event, so these properties and methods are always available

```C#
public abstract class HopFrameEventArgs {
    public TSender Sender { get; }
    public bool IsCanceled { get; }
    public TableConfig Table { get; }
    
    public void SetCancelled(bool canceled);
}
```

### Properties

- **Sender**: The sender of the event.
    - **Type:** `TSender`
- **IsCanceled**: Indicates whether the event is canceled.
    - **Type:** `bool`
- **Table**: The table configuration related to the event.
    - **Type:** `TableConfig`

### Methods

- **SetCancelled**
    - **Parameters:**
        - `canceled`: A boolean value to set the cancellation state.
    - **Returns:** `void`

## DeleteEntryEvent

Event arguments for a delete entry event.

```C#
public sealed class DeleteEntryEvent : HopFrameEventArgs {
    public object Entity { get; }
}
```

### Properties

- **Entity**: The entity being deleted.
    - **Type:** `object`

## CreateEntryEvent

Event arguments for a create entry event.

```C#
public sealed class CreateEntryEvent : HopFrameEventArgs {

}
```

## UpdateEntryEvent

Event arguments for an update entry event.

```C#
public sealed class UpdateEntryEvent : HopFrameEventArgs {
    public object Entity { get; }
}
```

### Properties

- **Entity**: The entity being updated.
    - **Type:** `object`

## SelectEntryEvent

Event arguments for a select entry event.

```C#
public sealed class SelectEntryEvent : HopFrameEventArgs {
    public object Entity { get; }
    public bool Selected { get; set; }
}
```

### Properties

- **Entity**: The entity being selected.
    - **Type:** `object`
- **Selected**: Indicates whether the entity is selected.
    - **Type:** `bool`

## PageChangeEvent

Event arguments for a page change event.

```C#
public sealed class PageChangeEvent : HopFrameEventArgs {
    public int CurrentPage { get; }
    public int TotalPages { get; }
    public int NewPage { get; set; }
}
```

### Properties

- **CurrentPage**: The current page number.
    - **Type:** `int`
- **TotalPages**: The total number of pages.
    - **Type:** `int`
- **NewPage**: The new page number to navigate to.
    - **Type:** `int`

## ReloadEvent

Event arguments for a reload event.

```C#
public sealed class ReloadEvent : HopFrameEventArgs {

}
```

## SearchEvent

Event arguments for a search event.

```C#
public sealed class SearchEvent : HopFrameEventArgs {
    public string SearchTerm { get; set; }
    public int CurrentPage { get; }
    
    public void SetSearchResult(IEnumerable result, int totalPages);
}
```

### Properties

- **SearchTerm**: The search term used.
    - **Type:** `string`
- **CurrentPage**: The current page number.
    - **Type:** `int`
- **SearchResult**: The search results.
    - **Type:** `IEnumerable<object>?`
- **TotalPages**: The total number of pages of search results.
    - **Type:** `int`

### Methods

- **SetSearchResult**
    - **Parameters:**
        - `result`: The current page of search results.
        - `totalPages`: The total pages of search results.
    - **Returns:** `void`

## TableInitializedEvent

Event arguments for a table initialization event.

```C#
public class TableInitializedEvent : HopFrameEventArgs {
    public List<PluginButton> PluginButtons { get; };
    public DefaultButtonToggles DefaultButtons { get; set; };
    
    public void AddPageButton(string title, Func<Task> callback, bool pushRight = false, IconInfo? icon = null);
    public void AddPageButton(string title, Action callback, bool pushRight = false, IconInfo? icon = null);
    public void AddPageButton<TEntity>(string title, Func<Task> callback, bool pushRight = false, IconInfo? icon = null);
    public void AddPageButton<TEntity>(string title, Action callback, bool pushRight = false, IconInfo? icon = null);

    public void AddEntityButton(IconInfo icon, Func<object, TableConfig, Task> callback);
    public void AddEntityButton(IconInfo icon, Action<object, TableConfig> callback);
    public void AddEntityButton<TEntity>(IconInfo icon, Func<TEntity, TableConfig, Task> callback);
    public void AddEntityButton<TEntity>(IconInfo icon, Action<TEntity, TableConfig> callback);
}
```

### Properties

- **PluginButtons**: The list of plugin buttons for the table.
    - **Type:** `List<PluginButton>`
- **DefaultButtons**: The default button toggles for the table.
    - **Type:** `DefaultButtonToggles`

### Methods

- **AddPageButton**
    - **Parameters:**
        - `title`: The title of the button.
        - `callback`: The callback function for the button.
        - `pushRight`: Indicates whether to push the button to the right. (default: `false`)
        - `icon`: The icon for the button. (default: `null`)
    - **Returns:** `void`

- **AddEntityButton**
    - **Parameters:**
        - `icon`: The icon for the button.
        - `callback`: The callback function for the button.
    - **Returns:** `void`

## ValidationEvent

Event arguments for a validation event.

```C#
public sealed class ValidationEvent : HopFrameEventArgs {
    public IList<string> Errors { get; }
    public PropertyConfig Property { get; }
}
```

### Properties

- **Errors**: The list of validation errors.
    - **Type:** `IList<string>`
- **Property**: The property being validated.
    - **Type:** `PropertyConfig`
