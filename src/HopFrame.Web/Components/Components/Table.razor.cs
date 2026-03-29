using System.ComponentModel;
using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HopFrame.Web.Components.Components;

public partial class Table(IEntityAccessor accessor, IConfigAccessor configAccessor) : ComponentBase {
    
    private readonly record struct TableEntry {
        public object Entry { get; init; }
        public Dictionary<string, string> Columns { get; init; }
    }
    
    [Parameter]
    public required TableConfig Config { get; set; }

    [Parameter]
    public EventCallback OnAdd { get; set; }

    [Parameter]
    public EventCallback<object> OnDelete { get; set; }

    [Parameter]
    public EventCallback<object> OnEdit { get; set; }

    [Parameter]
    public bool ShowActionButtons { get; set; } = true;

    [Parameter]
    public SelectionMode SelectionMode { get; set; } = SelectionMode.None;
    
    [Parameter]
    public List<object>? Preselected { get; set; }

    public HashSet<object> SelectedEntries { get; } = new();

    private IHopFrameRepository Repository { get; set; } = null!;

    private PropertyConfig[] OrderedProperties { get; set; } = null!;

    private MudTable<TableEntry> Manager { get; set; } = null!;

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

        if (Preselected is not null) {
            foreach (var entry in Preselected) {
                SelectedEntries.Add(entry);
            }
        }
    }

    public Task Reload() => Manager.ReloadServerData();

    private Dictionary<string, string> PrepareData(object entry) {
        var dict = new Dictionary<string, string>();
        foreach (var prop in OrderedProperties) {
            dict.Add(prop.Identifier, accessor.GetValue(entry, prop) ?? string.Empty);
        }

        return dict;
    }

    private async Task<TableData<TableEntry>> ReloadTable(TableState state, CancellationToken ct) {
        IEnumerable<object> entries;
        int total;

        var sort = new Sorting(null, ListSortDirection.Ascending);
        if (_currentSort.HasValue) {
            sort = new(_currentSort.Value.Key, _currentSort.Value.Value switch {
                SortDirection.Ascending => ListSortDirection.Ascending,
                SortDirection.Descending => ListSortDirection.Descending,
                _ => ListSortDirection.Ascending
            });
        }

        if (string.IsNullOrWhiteSpace(_searchText)) {
            entries = await Repository.LoadPageGenericAsync(state.Page, state.PageSize, sort, ct);
            total = await Repository.CountAsync(ct);
        }
        else {
            var result = await Repository.SearchGenericAsync(_searchText, state.Page, state.PageSize, sort, ct);
            entries = result.Result;
            total = result.PageCount;
        }

        var data = entries.Select(e => new TableEntry {
            Entry = e,
            Columns = PrepareData(e)
        }).ToArray();

        return new TableData<TableEntry> {
            TotalItems = total,
            Items = data
        };
    }

    private bool IsSelected(TableEntry entry) {
        return SelectedEntries.Contains(entry.Entry);
    }

    private void ToggleSelect(TableEntry entry) {
        if (SelectionMode == SelectionMode.Single) {
            if (IsSelected(entry)) return;
            
            SelectedEntries.Clear();
            SelectedEntries.Add(entry.Entry);
        }
        else {
            if (IsSelected(entry))
                SelectedEntries.Remove(entry.Entry);
            else
                SelectedEntries.Add(entry.Entry);
        }
    }

    private async Task OnSearch(string searchText) {
#pragma warning disable BL0005
        Manager.CurrentPage = 0;
#pragma warning restore BL0005
        _searchText = searchText;
        await Manager.ReloadServerData();
    }

    private bool _currentlyReloading;
    private async Task OnSort(PropertyConfig property, SortDirection direction) {
        if (_currentlyReloading) return;
        _currentlyReloading = true;
        
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
        _currentlyReloading = false;
    }

    private async Task OnAddClick() {
        if (OnAdd.HasDelegate)
            await OnAdd.InvokeAsync();
    }

    private async Task OnEditClick(object entry) {
        if (OnEdit.HasDelegate)
            await OnEdit.InvokeAsync(entry);
    }

    private async Task OnDeleteClick(object entry) {
        if (OnDelete.HasDelegate)
            await OnDelete.InvokeAsync(entry);
    }

    private void OnRowClick(TableRowClickEventArgs<TableEntry> e) {
        if (SelectionMode != SelectionMode.None) {
            ToggleSelect(e.Item);
        }
    }
}

public enum SelectionMode {
    None = 0,
    Single = 1,
    Multiple = 2
}
