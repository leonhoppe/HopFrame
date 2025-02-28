using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Web.Components.Pages;
using HopFrame.Web.Plugins.Annotations;
using HopFrame.Web.Plugins.Events;
using HopFrame.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;

namespace HopFrame.Web.Plugins.Internal;

internal sealed class ExporterPlugin(IContextExplorer explorer, IToastService toasts, IFileService files, IServiceProvider provider) {
    public const char Separator = ';';

    [EventHandler]
    public void OnInit(TableInitializedEvent e) {
        e.AddPageButton("Export", () => Export(e.Table), icon: new Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20.ArrowUpload());
        e.AddPageButton("Import", () => Import(e.Table, e.Sender), icon: new Microsoft.FluentUI.AspNetCore.Components.Icons.Regular.Size20.ArrowDownload());
    }

    private async Task Export(TableConfig table) {
        var manager = explorer.GetTableManager(table.PropertyName);
        if (manager is null) {
            toasts.ShowError("Data could not be exported!");
            return;
        }

        var data = await manager
            .LoadPage(0, int.MaxValue)
            .ToArrayAsync();

        var properties = table.Properties.Where(prop => !prop.IsVirtualProperty).ToArray();
        
        var csv = new StringBuilder(string.Join(Separator, properties.Select(prop => prop.Info.Name)) + '\n');
        foreach (var entry in data) {
            var row = new List<string>();
            
            foreach (var property in properties) {
                row.Add(FormatProperty(property, entry));
            }

            csv.Append(string.Join(Separator, row) + '\n');
        }

        var result = csv.ToString();
        await files.DownloadFile($"{table.DisplayName}.csv", Encoding.UTF8.GetBytes(result));
    }

    private async Task Import(TableConfig table, HopFrameTablePage target) {
        var file = await files.UploadFile();
        
        var stream = file.OpenReadStream();
        var reader = new StreamReader(stream);
        
        var properties = table.Properties.Where(prop => !prop.IsVirtualProperty).ToArray();
        var data = await reader.ReadToEndAsync();
        var rows = data.Split('\n');
        
        reader.Dispose();
        await stream.DisposeAsync();

        var headerProps = rows.First().Split(Separator);
        if (!headerProps.Any(h => properties.Any(prop => prop.Info.Name == h))) {
            toasts.ShowError("Table header in csv is not valid!");
            return;
        }

        var elements = new List<object>();
        for (int rowIndex = 1; rowIndex < rows.Length; rowIndex++) {
            var row = rows[rowIndex];
            if (string.IsNullOrWhiteSpace(row)) continue;
            
            var element = Activator.CreateInstance(table.TableType)!;
            
            var rowValues = row.Split(Separator);
            for (int i = 0; i < headerProps.Length; i++) {
                var property = properties.FirstOrDefault(prop => prop.Info.Name == headerProps[i]);
                if (property is null) continue;

                object? value = rowValues[i];
                
                if (property.IsEnumerable) {
                    if (!property.Info.PropertyType.IsGenericType) continue;
                    
                    var formattedEnumerable = (string)value;
                    if (formattedEnumerable == "[]") continue;
                    var values = formattedEnumerable
                        .TrimStart('[')
                        .TrimEnd(']')
                        .Split(',');

                    var addMethod = property.Info.PropertyType.GetMethod("Add");
                    if (addMethod is null) continue;

                    var tableType = property.Info.PropertyType.GenericTypeArguments[0];
                    var relationManager = explorer.GetTableManager(tableType);
                    var primaryKeyType = GetPrimaryKeyType(tableType);
                    if (relationManager is null || primaryKeyType is null) continue;

                    var enumerable = Activator.CreateInstance(property.Info.PropertyType);
                    foreach (var key in values) {
                        var entry = await relationManager.GetOne(ParseString(key, primaryKeyType));
                        if (entry is null) continue;

                        addMethod.Invoke(enumerable, [entry]);
                    }
                    
                    property.Info.SetValue(element, enumerable);
                    continue;
                }

                if (property.IsRelation) {
                    var relationManager = explorer.GetTableManager(property.Info.PropertyType);
                    var relationPrimaryKeyType = GetPrimaryKeyType(property.Info.PropertyType);
                    if (relationManager is null || relationPrimaryKeyType is null) continue;
                    value = await relationManager.GetOne(ParseString((string)value, relationPrimaryKeyType));
                }
                else if (property.Info.PropertyType == typeof(Guid)) {
                    var success = Guid.TryParse((string)value, out var guid);
                    if (success) value = guid;
                    else toasts.ShowError($"'{value}' is not a valid guid");
                }
                else {
                    value = ParseString((string)value, property.Info.PropertyType);
                }
                
                property.Info.SetValue(element, value);
            }
            
            elements.Add(element);
        }
        
        var manager = explorer.GetTableManager(table.PropertyName);
        if (manager is null) {
            toasts.ShowError("Data could not be imported!");
            return;
        }

        await manager.AddAll(elements);
        await target.Reload();
    }

    private string FormatProperty(PropertyConfig property, object entity) {
        var value = property.Info.GetValue(entity);

        if (value is null)
            return string.Empty;

        if (property.IsEnumerable) {
            var enumerable = (IEnumerable)value;
            return '[' + string.Join(',', enumerable.OfType<object>().Select(o => SelectPrimaryKey(o) ?? o.ToString())) + ']';
        }

        return SelectPrimaryKey(value) ?? value.ToString() ?? string.Empty;
    }

    private string? SelectPrimaryKey(object entity) {
        return entity
            .GetType()
            .GetProperties()
            .FirstOrDefault(prop => prop
                .GetCustomAttributes(true)
                .Any(attr => attr is KeyAttribute))?
            .GetValue(entity)?
            .ToString();
    }

    private Type? GetPrimaryKeyType(Type tableType) {
        return tableType
            .GetProperties()
            .FirstOrDefault(prop => prop
                .GetCustomAttributes(true)
                .Any(attr => attr is KeyAttribute))?
            .PropertyType;
    }

    private object? ParseString(string input, Type targetType) {
        try {
            var parseMethod = targetType
                .GetMethods()
                .Where(method => method.Name.StartsWith("Parse"))
                .FirstOrDefault(method => method.GetParameters().SingleOrDefault()?.ParameterType == typeof(string));

            if (parseMethod is not null)
                return parseMethod.Invoke(null, [input]);
            
            return Convert.ChangeType(input, targetType);
        }
        catch (Exception) {
            return null;
        }
    }
    
}