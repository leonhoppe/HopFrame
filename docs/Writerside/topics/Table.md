# Table

On the table page you can view, edit, delete and create entries in that table.
You can use the many configuration methods provided by the [](TableConfig.md) to modify the look
of this page.

An example configuration could look something like this:

![table.png](table.png)

Here you can use the various buttons to interact with your entities.

## Data grid

The main aspect of this site is the data grid that displays all your entries of that table.
For performance reasons this data is paginated, you can **change the page** at the bottom of the screen
using either the arrow button or the page selector. You can **sort** your entries by clicking on
the name of the column. If you don't want a column to be sortable, you can configure this in the
[PropertyConfig](PropertyConfig.md#issortable).

## Search

The search bar will search through all entities in your database, not only the ones displayed.
Simply enter a search term and the search is performed automatically. You can configure the
columns that should be searchable in the [PropertyConfig](PropertyConfig.md#issearchable)

## Edit dialog

If you click on the pen button next to one of the entries, the edit dialog will appear,
it could look something like this:

![editor.png](editor.png)

You can modify the data of the selected entity based on your [PropertyConfigs](PropertyConfig.md).
The HopFrame can handle various property types like strings, numbers, enums, relations and many more.

### Validation

The HopFrame also supports input validation. By default, a required validation is automatically applied
to every property that's not nullable. You can change the validation behavior in the
[PropertyConfig](PropertyConfig.md#setvalidator-synchronous).
