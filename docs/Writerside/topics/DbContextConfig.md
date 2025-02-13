# DbContextConfig

This config contains all configurations for the given DbContext type.

## Configuration methods

### Table (With configurator)

Configures the table of the `DbContext` using the provided configurator.

```c#
DbContextConfigurator<TDbContext> Table<TModel>(Action<TableConfigurator<TModel>> configurator) where TModel : class
```

- **Type Parameters:**
    - `TModel`: The model of the table for identifying the correct one.

- **Parameters:**
    - `configurator`: Used for configuring the table.

- **Returns:** `DbContextConfigurator<TDbContext>`

- **See Also:** [](TableConfig.md)

### Table (Without configurator)

Configures the table of the `DbContext`.

```c#
TableConfigurator<TModel> Table<TModel>() where TModel : class
```

- **Type Parameters:**
    - `TModel`: The model of the table for identifying the correct one.

- **Returns:** `TableConfigurator<TModel>`

- **See Also:** [](TableConfig.md)
