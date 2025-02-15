using System.Text;
using HopFrame.Core.Config;
using HopFrame.Core.Services;
using HopFrame.Web.Components.Pages;
using HopFrame.Web.Plugins.Annotations;
using HopFrame.Web.Plugins.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;

namespace HopFrame.Web.Plugins.Internal;

internal sealed class ExporterPlugin(IContextExplorer explorer, IJSRuntime runtime, IToastService toasts, IServiceProvider provider) : HopFramePlugin {
    
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

        var properties = table.Properties.Where(prop => prop.List).OrderBy(prop => prop.Order).ToArray();

        
        var csv = new StringBuilder(string.Join(Separator, properties.Select(prop => prop.Name)) + '\n');
        foreach (var entry in data) {
            var row = new List<string>();
            
            foreach (var property in properties) {
                var value = await manager.DisplayProperty(entry, property);
                row.Add(value);
            }

            csv.Append(string.Join(Separator, row) + '\n');
        }

        var result = csv.ToString();
        await DownloadFile($"{table.DisplayName}.csv", Encoding.UTF8.GetBytes(result), runtime);
    }

    private async Task Import(TableConfig table, HopFrameTablePage target) {
        var file = await UploadFile(target, runtime);
        
        var stream = file.OpenReadStream();
        var reader = new StreamReader(stream);
        
        var properties = table.Properties.Where(prop => prop.List).OrderBy(prop => prop.Order).ToArray();
        var data = await reader.ReadToEndAsync();
        var rows = data.Split('\n');
        
        reader.Dispose();
        await stream.DisposeAsync();

        var headerProps = rows.First().Split(Separator);
        if (!headerProps.Any(h => properties.Any(prop => prop.Name == h))) {
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
                var property = properties.FirstOrDefault(prop => prop.Name == headerProps[i]);
                if (property is null) continue;
                if (property.IsEnumerable) continue;

                object value = rowValues[i];
                
                if (property.Info.PropertyType == typeof(Guid)) {
                    var success = Guid.TryParse((string)value, out var guid);
                    if (success) value = guid;
                    else toasts.ShowError($"'{value}' is not a valid guid");
                }
                
                if (property.Parser is not null) {
                    value = await property.Parser(value.ToString()!, provider);
                }
                property.SetValue(element, value, provider);
            }
            
            elements.Add(element);
        }
        
        var manager = explorer.GetTableManager(table.PropertyName);
        if (manager is null) {
            toasts.ShowError("Data could not be exported!");
            return;
        }

        await manager.AddAll(elements);
        await target.Reload();
    }
    
}