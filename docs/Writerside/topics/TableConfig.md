# TableConfig

This configuration contains all configurations for the given table type.

## Configuration methods

### Ignore

Determines if the table should be ignored in the admin UI.

```c#
TableConfigurator<TModel> Ignore(bool ignore)
```

- **Parameters:**
    - `ignore`: A boolean value to set if the table should be ignored.

- **Returns:** `TableConfigurator<TModel>`

### Property (With configurator)

Configures the property of the table using the provided configurator.

```c#
TableConfigurator<TModel> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression, Action<PropertyConfigurator<TProp>> configurator)
```

- **Parameters:**
    - `propertyExpression`: Used for determining the property.
    - `configurator`: Used for configuring the property.

- **Returns:** `TableConfigurator<TModel>`

- **See Also:** [](PropertyConfig.md)

### Property (Without configurator)

Configures the property of the table.

```c#
PropertyConfigurator<TProp> Property<TProp>(Expression<Func<TModel, TProp>> propertyExpression)
```

- **Parameters:**
    - `propertyExpression`: Used for determining the property.

- **Returns:** `PropertyConfigurator<TProp>`

- **See Also:** [](PropertyConfig.md)

### AddVirtualProperty (With configurator)

Adds a virtual property to the table view and configures it using the provided configurator (this property will not appear in the editor).

```c#
TableConfigurator<TModel> AddVirtualProperty(string name, Func<TModel, IServiceProvider, string> template, Action<PropertyConfigurator<string>> configurator)
```

- **Parameters:**
    - `name`: The name of the virtual property.
    - `template`: The template used for generating the property value.
    - `configurator`: Used for configuring the virtual property.

- **Returns:** `TableConfigurator<TModel>`

- **See Also:** [](PropertyConfig.md)

### AddVirtualProperty (Synchronous)

Adds a virtual property to the table view (this property will not appear in the editor).

```c#
PropertyConfigurator<string> AddVirtualProperty(string name, Func<TModel, IServiceProvider, string> template)
```

- **Parameters:**
    - `name`: The name of the virtual property.
    - `template`: The template used for generating the property value.

- **Returns:** `PropertyConfigurator<string>`

- **See Also:** [](PropertyConfig.md)

### AddVirtualProperty (Asynchronous)

Adds a virtual property to the table view (this property will not appear in the editor).

```c#
PropertyConfigurator<string> AddVirtualProperty(string name, Func<TModel, IServiceProvider, Task<string>> template)
```

- **Parameters:**
    - `name`: The name of the virtual property.
    - `template`: The template used for generating the property value.

- **Returns:** `PropertyConfigurator<string>`

- **See Also:** [](PropertyConfig.md)

### SetDisplayName

Determines the name for the table used in the admin UI and URL for the table page.

```c#
TableConfigurator<TModel> SetDisplayName(string name)
```

- **Parameters:**
    - `name`: The display name for the table.

- **Returns:** `TableConfigurator<TModel>`

### SetDescription

Determines the description displayed in the admin UI.

```c#
TableConfigurator<TModel> SetDescription(string description)
```

- **Parameters:**
    - `description`: The description for the table.

- **Returns:** `TableConfigurator<TModel>`

### SetOrderIndex

Determines the order index for the table in the admin UI.

```c#
TableConfigurator<TModel> SetOrderIndex(int index)
```

- **Parameters:**
    - `index`: The order index for the table.

- **Returns:** `TableConfigurator<TModel>`

- **See Also:** [](PropertyConfig.md#setorderindex)

### SetViewPolicy

Determines the policy needed by a user in order to view the table.

```c#
TableConfigurator<TModel> SetViewPolicy(string policy)
```

- **Parameters:**
    - `policy`: The view policy string.

- **Returns:** `TableConfigurator<TModel>`

### SetUpdatePolicy

Determines the policy needed by a user in order to edit the entries.

```c#
TableConfigurator<TModel> SetUpdatePolicy(string policy)
```

- **Parameters:**
    - `policy`: The update policy string.

- **Returns:** `TableConfigurator<TModel>`

### SetCreatePolicy

Determines the policy needed by a user in order to create entries.

```c#
TableConfigurator<TModel> SetCreatePolicy(string policy)
```

- **Parameters:**
    - `policy`: The create policy string.

- **Returns:** `TableConfigurator<TModel>`

### SetDeletePolicy

Determines the policy needed by a user in order to delete entries.

```c#
TableConfigurator<TModel> SetDeletePolicy(string policy)
```

- **Parameters:**
    - `policy`: The delete policy string.

- **Returns:** `TableConfigurator<TModel>`
