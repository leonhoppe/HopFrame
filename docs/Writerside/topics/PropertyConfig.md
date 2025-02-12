# PropertyConfig

This configuration contains all configurations for the given property type on the table.

## Configuration methods

### SetDisplayName

Sets the title displayed in the table header and edit dialog.

```c#
PropertyConfigurator<TProp> SetDisplayName(string displayName)
```

- **Parameters:**
    - `displayName`: The display name for the property.

- **Returns:** `PropertyConfigurator<TProp>`

### List

Determines if the property should appear in the table, if not the property is also set to be not searchable.

```c#
PropertyConfigurator<TProp> List(bool list)
```

- **Parameters:**
    - `list`: A boolean value to set if the property should appear in the table.

- **Returns:** `PropertyConfigurator<TProp>`

### IsSortable

Determines if the table can be sorted by the property.

```c#
PropertyConfigurator<TProp> IsSortable(bool sortable)
```

- **Parameters:**
    - `sortable`: A boolean value to set if the property is sortable.

- **Returns:** `PropertyConfigurator<TProp>`

### IsSearchable

Determines if the property get taken into account for search results.

```c#
PropertyConfigurator<TProp> IsSearchable(bool searchable)
```

- **Parameters:**
    - `searchable`: A boolean value to set if the property is searchable.

- **Returns:** `PropertyConfigurator<TProp>`

### SetDisplayedProperty

Determines if the value that should be displayed instead of the string representation of the type.

```c#
PropertyConfigurator<TProp> SetDisplayedProperty<TInnerProp>(Expression<Func<TProp, TInnerProp>> propertyExpression)
```

- **Type Parameters:**
    - `TInnerProp`: The inner property type to display.

- **Parameters:**
    - `propertyExpression`: The expression to determine the property.

- **Returns:** `PropertyConfigurator<TProp>`

### Format (Synchronous)

Determines the value that's displayed in the admin UI.

```c#
PropertyConfigurator<TProp> Format(Func<TProp, IServiceProvider, string> formatter)
```

- **Parameters:**
    - `formatter`: The function to format the value.

- **Returns:** `PropertyConfigurator<TProp>`

- **See Also:** [](#setdisplayedproperty)

### Format (Asynchronous)

Determines the value that's displayed in the admin UI.

```c#
PropertyConfigurator<TProp> Format(Func<TProp, IServiceProvider, Task<string>> formatter)
```

- **Parameters:**
    - `formatter`: The function to format the value.

- **Returns:** `PropertyConfigurator<TProp>`

### FormatEach (Synchronous)

Determines the value that's displayed for each entry in the list.

```c#
PropertyConfigurator<TProp> FormatEach<TInnerProp>(Func<TInnerProp, IServiceProvider, string> formatter)
```

- **Parameters:**
    - `formatter`: The function to format the value for each entry.

- **Returns:** `PropertyConfigurator<TProp>`

### FormatEach (Asynchronous)

Determines the value that's displayed for each entry in the list.

```c#
PropertyConfigurator<TProp> FormatEach<TInnerProp>(Func<TInnerProp, IServiceProvider, Task<string>> formatter)
```

- **Parameters:**
    - `formatter`: The function to format the value for each entry.

- **Returns:** `PropertyConfigurator<TProp>`

### SetParser (Synchronous)

Determines the function used for parsing the value provided in the editor dialog to the actual property value.

```c#
PropertyConfigurator<TProp> SetParser(Func<string, IServiceProvider, TProp> parser)
```

- **Parameters:**
    - `parser`: The function to parse the value.

- **Returns:** `PropertyConfigurator<TProp>`

### SetParser (Asynchronous)

Determines the function used for parsing the value provided in the editor dialog to the actual property value.

```c#
PropertyConfigurator<TProp> SetParser(Func<string, IServiceProvider, Task<TProp>> parser)
```

- **Parameters:**
    - `parser`: The function to parse the value.

- **Returns:** `PropertyConfigurator<TProp>`

### SetEditable

Determines if the value can be edited in the admin UI. If true, the value can still be initially set, but not modified.

```c#
PropertyConfigurator<TProp> SetEditable(bool editable)
```

- **Parameters:**
    - `editable`: A boolean value to set if the property is editable.

- **Returns:** `PropertyConfigurator<TProp>`

### SetCreatable

Determines if the initial value can be edited in the admin UI. If true the value will not be visible in the create dialog.

```c#
PropertyConfigurator<TProp> SetCreatable(bool creatable)
```

- **Parameters:**
    - `creatable`: A boolean value to set if the property is creatable.

- **Returns:** `PropertyConfigurator<TProp>`

### DisplayValue

Determines if the value should be displayed in the admin UI (useful for secrets like passwords etc.).

```c#
PropertyConfigurator<TProp> DisplayValue(bool display)
```

- **Parameters:**
    - `display`: A boolean value to set if the property value is displayed.

- **Returns:** `PropertyConfigurator<TProp>`

### IsTextArea

Determines if the admin UI should use a text area for modifying the value.

```c#
PropertyConfigurator<TProp> IsTextArea(bool textField)
```

- **Parameters:**
    - `textField`: A boolean value to set if the property is a text area.

- **Returns:** `PropertyConfigurator<TProp>`

### SetTextAreaRows

Determines the initial size of the text area field.

```c#
PropertyConfigurator<TProp> SetTextAreaRows(int rows)
```

- **Parameters:**
    - `rows`: The number of rows for the text area.

- **Returns:** `PropertyConfigurator<TProp>`

### SetValidator (Synchronous)

Determines the validator used for the property value before saving.

```c#
PropertyConfigurator<TProp> SetValidator(Func<TProp?, IServiceProvider, IEnumerable<string>> validator)
```

- **Parameters:**
    - `validator`: The function to validate the property value.

- **Returns:** `PropertyConfigurator<TProp>`

### SetValidator (Asynchronous)

Determines the validator used for the property value before saving.

```c#
PropertyConfigurator<TProp> SetValidator(Func<TProp?, IServiceProvider, Task<IEnumerable<string>>> validator)
```

- **Parameters:**
    - `validator`: The function to validate the property value.

- **Returns:** `PropertyConfigurator<TProp>`

### SetOrderIndex

Determines the order index for the property in the admin UI.

```c#
PropertyConfigurator<TProp> SetOrderIndex(int index)
```

- **Parameters:**
    - `index`: The order index for the property.

- **Returns:** `PropertyConfigurator<TProp>`

- **See Also:** [](TableConfig.md#setorderindex)

### SetDisplayLength

Sets the maximum character length displayed in the admin UI (not in the editor dialog).

```c#
PropertyConfigurator<TProp> SetDisplayLength(int maxLength)
```

- **Parameters:**
    - `maxLength`: The maximum length of characters to be displayed.

- **Returns:** `PropertyConfigurator<TProp>`

### ForceRelation

Forces a property to be treated as a relation.

```c#
PropertyConfigurator<TProp> ForceRelation(bool isEnumerable = false, bool isRequired = true)
```

- **Parameters:**
    - `isEnumerable`: Determines if it is possible to assign multiple objects to the property.
    - `isRequired`: Determines if the property is nullable.

- **Returns:** `PropertyConfigurator<TProp>`