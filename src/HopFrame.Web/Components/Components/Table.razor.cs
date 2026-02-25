using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HopFrame.Web.Components.Components;

public partial class Table(IEntityAccessor accessor, IConfigAccessor configAccessor) : ComponentBase {
    
    [Parameter]
    public required TableConfig Config { get; set; }

    private IHopFrameRepository Repository { get; set; } = null!;

    private PropertyConfig[] OrderedProperties { get; set; } = null!;

    private MudTable<Dictionary<string, string>> Manager { get; set; } = null!;

    protected override void OnInitialized() {
        base.OnInitialized();

        Repository = configAccessor.LoadRepository(Config);

        OrderedProperties = Config.Properties
            .Where(p => p.Listable)
            .OrderBy(p => p.OrderIndex)
            .ToArray();
    }

    private async Task<List<Dictionary<string, string>>> PrepareData(object[] entries) {
        var list = new List<Dictionary<string, string>>();
        
        foreach (var entry in entries) {
            var taskDict = new Dictionary<string, Task<string?>>();
            foreach (var prop in OrderedProperties) {
                taskDict.Add(prop.Identifier, accessor.GetValue(entry, prop));
            }

            await Task.WhenAll(taskDict.Values);

            var dict = new Dictionary<string, string>();
            foreach (var prop in taskDict) {
                dict.Add(prop.Key, prop.Value.Result ?? string.Empty);
            }
            
            list.Add(dict);
        }

        return list;
    }

    private async Task<TableData<Dictionary<string, string>>> Reload(TableState state, CancellationToken ct) {
        var entries = await Repository.LoadPageGenericAsync(state.Page, state.PageSize, ct);
        var data = await PrepareData(entries.Cast<object>().ToArray());
        var total = await Repository.CountAsync(ct);

        return new TableData<Dictionary<string, string>> {
            TotalItems = total,
            Items = data
        };
    }

    private async Task OnSearch(string searchText) {
        Console.WriteLine(searchText);
    }
    
}