# Exporter Plugin

The Exporter Plugin is a tool for managing the import and export of data from the HopFrame UI. It provides functionality for exporting table data into a CSV file and importing data back into the system, making data manipulation and backups more seamless.

## What the Exporter Plugin Does

1. **Export Table Data to CSV**
    - The plugin allows users to export all data from a table as a CSV file.
    - The exported file includes all non-virtual properties as table headers.
    - The export process dynamically constructs rows for each entry in the table.

2. **Import Data from CSV**
    - Users can import a CSV file to populate or update a table.
    - The import process reads the file, validates the headers, and creates new entries or updates existing ones.
    - Relationships and enumerable properties are also resolved using the appropriate managers.

3. **User Interface Integration**
    - Adds two buttons, "Export" and "Import," to the page header of each table.
        - **Export Button:** Initiates the export functionality.
        - **Import Button:** Allows users to upload a CSV file for import.

4. **Error Handling**
    - Ensures errors during import or export (e.g., invalid file format, missing data, or system issues) are shown to the user as toast messages.

## Adding the Exporter Plugin

To include the Exporter Plugin in your HopFrame setup, use the `AddExporters` method provided by the `HopFrameConfiguratorExtensions`.

Here’s how to register the Exporter Plugin in your application configuration:

```c#
builder.Services.AddHopFrame(options => {
    options.AddExporters();
});
```

The `AddExporters` method internally registers the `ExporterPlugin` and attaches its functionality to the HopFrame.

## Key Features of the Export Process

- **Dynamic Header Creation:** Automatically generates headers based on the table's non-virtual properties.
- **Data Transformation:** Transforms property values into CSV-compatible formats.
- **File Download:** Saves the generated CSV file with the table’s display name.

## Key Features of the Import Process

- **Header Validation:** Validates that the CSV file headers match the table's properties.
- **Type Conversion:** Converts values in the CSV file to their respective data types.
- **Relationship Management:** Resolves relationships and enumerable properties during import.

This plugin streamlines data operations, reducing manual effort and enabling quick data migration or updates. Let me know if you’d like to dive deeper into any specific aspect!