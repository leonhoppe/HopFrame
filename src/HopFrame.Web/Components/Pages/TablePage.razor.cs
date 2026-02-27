using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using HopFrame.Web.Components.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HopFrame.Web.Components.Pages;

public partial class TablePage(IConfigAccessor accessor, NavigationManager navigator, IDialogService dialogs, ISnackbar snackbar) : CancellableComponent {
    
    [Parameter]
    public string TableRoute { get; set; } = null!;

    private TableConfig Table { get; set; } = null!;

    private IHopFrameRepository Repository { get; set; } = null!;

    private Table TableComponent { get; set; } = null!;

    private Editor EditorComponent { get; set; } = null!;

    protected override void OnInitialized() {
        base.OnInitialized();

        var table = accessor.GetTableByRoute(TableRoute);

        if (table is null) {
            navigator.NavigateTo("/admin", true);
            return;
        }

        Table = table;
        Repository = accessor.LoadRepository(table);
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
    }

    private async Task OnAdd() {
        var entry = await EditorComponent.Present(null);
        if (entry is null) return;
        
        await Repository.CreateGenericAsync(entry, TokenSource.Token);
        await TableComponent.Reload();
        snackbar.Add("Entry added", Severity.Success);
    }

    private async Task OnEdit(object entry) {
        var newEntry = await EditorComponent.Present(entry);
        if (newEntry is null) return;
        
        await Repository.UpdateGenericAsync(newEntry, TokenSource.Token);
        await TableComponent.Reload();
        snackbar.Add("Entry updated", Severity.Success);
    }

    private async Task OnDelete(object entry) {
        var dialog = await dialogs.ShowAsync<DeleteConfirmationDialog>();
        var result = await dialog.Result;

        if (result is not null && !result.Canceled) {
            await Repository.DeleteGenericAsync(entry, TokenSource.Token);
            await TableComponent.Reload();
            snackbar.Add("Entry deleted", Severity.Success);
        }
    }
}