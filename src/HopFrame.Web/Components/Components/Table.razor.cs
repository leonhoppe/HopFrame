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

    private Dictionary<string, MudTableSortLabel<object>> SortDirections { get; set; } = new();

    private KeyValuePair<string, SortDirection>? _currentSort;

    private string _searchText = string.Empty;

    protected override void OnInitialized() {
        base.OnInitialized();

        Repository = configAccessor.LoadRepository(Config);

        OrderedProperties = Config.Properties
            .Where(p => p.Listable)
            .OrderBy(p => p.OrderIndex)
            .ToArray();
        
        foreach (var property in OrderedProperties) {
            SortDirections.Add(property.Identifier, null!);
        }
    }

    private List<Dictionary<string, string>> PrepareData(object[] entries) {
        var list = new List<Dictionary<string, string>>();
        
        foreach (var entry in entries) {
            var dict = new Dictionary<string, string>();
            foreach (var prop in OrderedProperties) {
                dict.Add(prop.Identifier, accessor.GetValue(entry, prop) ?? string.Empty);
            }
            
            list.Add(dict);
        }

        return list;
    }

    private async Task<TableData<Dictionary<string, string>>> Reload(TableState state, CancellationToken ct) {
        IEnumerable<object> entries;

        if (string.IsNullOrWhiteSpace(_searchText))
            entries = await Repository.LoadPageGenericAsync(state.Page, state.PageSize, ct);
        else
            entries = await Repository.SearchGenericAsync(_searchText, state.Page, state.PageSize, ct);

        if (_currentSort.HasValue) {
            var sortProp = Config.Properties.First(p => p.Identifier == _currentSort.Value.Key);
            entries = accessor.SortDataByProperty(entries, sortProp, _currentSort.Value.Value == SortDirection.Descending);
        }
        
        var data = PrepareData(entries.ToArray());
        var total = await Repository.CountAsync(ct);

        return new TableData<Dictionary<string, string>> {
            TotalItems = total,
            Items = data
        };
    }

    private async Task OnSearch(string searchText) {
        _searchText = searchText;
        await Manager.ReloadServerData();
    }

    private async Task OnSort(PropertyConfig property, SortDirection direction) {
        if (direction != SortDirection.None) {
            foreach (var reference in SortDirections
                         .Where(d => d.Key != property.Identifier)) {
#pragma warning disable BL0005
                reference.Value.SortDirection = SortDirection.None;
#pragma warning restore BL0005
            }
        }
        
        if (direction == SortDirection.None) {
            _currentSort = null;
        }
        else {
            _currentSort = new(property.Identifier, direction);
        }

        await Manager.ReloadServerData();
    }
}