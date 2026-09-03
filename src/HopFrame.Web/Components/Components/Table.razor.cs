using System.ComponentModel;
using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HopFrame.Web.Components.Components;

public partial class Table(IEntityAccessor accessor, IConfigAccessor configAccessor, ISnackbar snackbar, ILogger<Table> logger) : ComponentBase {
    
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

    private bool ShowAdvancedSearch { get; set; }

    private KeyValuePair<string, SortDirection>? _currentSort;

    private string _searchText = string.Empty;

    private List<AdvancedSearchProperty> _advancedSearchProperties = new();
    
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

        var presorted = Config.Properties.FirstOrDefault(p => p.Presorted is not null);
        if (presorted is not null) {
            var direction = presorted.Presorted switch {
                ListSortDirection.Ascending => SortDirection.Ascending,
                ListSortDirection.Descending => SortDirection.Descending,
                _ => SortDirection.None
            };

            _currentSort = new(presorted.Identifier, direction);
        }

        logger.LogDebug("Table component initialized for table '{table}'", Config.DisplayName);
    }

    protected override void OnAfterRender(bool firstRender) {
        if (!firstRender) return;
        var presorted = Config.Properties.FirstOrDefault(p => p.Presorted is not null);
        if (presorted is not null) {
#pragma warning disable BL0005
            SortDirections[presorted.Identifier].SortDirection = presorted.Presorted switch {
                ListSortDirection.Ascending => SortDirection.Ascending,
                ListSortDirection.Descending => SortDirection.Descending,
                _ => SortDirection.None
            };
#pragma warning restore BL000
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
        try {
            logger.LogDebug("Table reload was initiated for '{table}'", Config.DisplayName);
            IEnumerable<object> entries;
            int total;

            var sort = new Sorting(null, ListSortDirection.Ascending);
            if (_currentSort.HasValue) {
                sort = new(_currentSort.Value.Key, _currentSort.Value.Value switch {
                    SortDirection.Ascending => ListSortDirection.Ascending,
                    SortDirection.Descending => ListSortDirection.Descending,
                    _ => ListSortDirection.Ascending
                });

                logger.LogDebug("The new sort direction for table '{table}' is {sort} on property '{prop}'", Config.DisplayName, sort.Direction, sort.PropertyIdentifier);
            }

            if (_advancedSearchProperties.Any()) {
                var result = await Repository.SearchGenericAsync(_advancedSearchProperties, state.Page, state.PageSize, sort, ct);
                entries = result.Result;
                total = result.PageCount;
                logger.LogDebug("A total of {total} entries were found for table {table} with search query '{search}', displaying {amount} entries on page {page}", total, Config.DisplayName, _searchText, state.PageSize, state.Page);
            }
            else if (string.IsNullOrWhiteSpace(_searchText)) {
                entries = await Repository.LoadPageGenericAsync(state.Page, state.PageSize, sort, ct);
                total = await Repository.CountAsync(ct);
                logger.LogDebug("A total of {total} entries were found for table {table}, displaying {amount} entries on page {page}", total, Config.DisplayName, state.PageSize, state.Page);
            }
            else {
                var result = await Repository.SearchGenericAsync(_searchText, state.Page, state.PageSize, sort, ct);
                entries = result.Result;
                total = result.PageCount;
                logger.LogDebug("A total of {total} entries were found for table {table} with search query '{search}', displaying {amount} entries on page {page}", total, Config.DisplayName, _searchText, state.PageSize, state.Page);
            }

            var data = entries.Select(e => new TableEntry {
                Entry = e,
                Columns = PrepareData(e)
            }).ToArray();

            logger.LogDebug("A total of {amount} entries were prepared to be displayed on table '{table}'", data.Length, Config.DisplayName);

            return new TableData<TableEntry> {
                TotalItems = total,
                Items = data
            };
        } catch(Exception e) {
            logger.LogError(e, "An error occured while trying to display the table '{table}'", Config.DisplayName);
            snackbar.Add($"An error occured", Severity.Error);
            return new() {
                TotalItems = 0,
                Items = Array.Empty<TableEntry>()
            };
        }
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

    private async Task OnAdvancedSearch(string identifier, AdvancedSearchProperty? property) {
        var oldValue = _advancedSearchProperties.FirstOrDefault(p => p.Identifier == identifier);
        if (oldValue.Identifier == identifier)
            _advancedSearchProperties.Remove(oldValue);

        if (property.HasValue)
            _advancedSearchProperties.Add(property.Value);

#pragma warning disable BL0005
        Manager.CurrentPage = 0;
#pragma warning restore BL0005
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
