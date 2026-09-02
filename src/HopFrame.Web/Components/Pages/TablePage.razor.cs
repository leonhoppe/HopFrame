using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using HopFrame.Web.Components.Components;
using HopFrame.Web.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace HopFrame.Web.Components.Pages;

public partial class TablePage(IConfigAccessor accessor, NavigationManager navigator, IDialogService dialogs, ISnackbar snackbar, IAuthProvider authProvider, IEventEmitter eventEmitter, ILogger<TablePage> logger) : CancellableComponent {
    
    [Parameter]
    public string TableRoute { get; set; } = null!;

    private TableConfig Table { get; set; } = null!;

    private IHopFrameRepository Repository { get; set; } = null!;

    private Table TableComponent { get; set; } = null!;

    private Editor EditorComponent { get; set; } = null!;

    private bool ShowActionButtons { get; set; }

    protected override async Task OnInitializedAsync() {
        logger.LogDebug("Initializing table page for route '{route}'", TableRoute);
        var table = accessor.GetTableByRoute(TableRoute);

        if (table is null) {
            logger.LogDebug("Table for route '{route}' could not be found, redirecting to dashboard...", TableRoute);
            navigator.NavigateTo("/admin", true);
            return;
        }
        
        Table = table;
        logger.LogDebug("Found Table '{table}' for route '{route}'", Table.DisplayName, TableRoute);
        
        var authorized = await authProvider.IsAuthenticated(Table.ViewClaim, TokenSource.Token);
        
        if (!authorized) {
            logger.LogDebug("User is not authenticated to view '{route}' redirecting to dashboard...", TableRoute);
            navigator.NavigateTo("/admin", true);
            return;
        }
        
        ShowActionButtons = await authProvider.IsAuthenticated(Table.EditClaim, TokenSource.Token);
        Repository = accessor.LoadRepository(Table);
        snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        logger.LogDebug("Loaded repository '{rep}' for table '{table}'", Repository.GetType().FullName, Table.DisplayName);
    }

    private async Task OnAdd() {
        logger.LogDebug("Received call for entry adding on table '{table}'", Table.DisplayName);
        var entry = await EditorComponent.Present(null);
        if (entry is null) return;

        var authorized = await authProvider.IsAuthenticated(Table.EditClaim, TokenSource.Token);
        if (!authorized)
            return;
        
        logger.LogDebug("User is authorized to add an entry on table '{table}'", Table.DisplayName);
        
        try {
            await Repository.CreateGenericAsync(entry, TokenSource.Token);
            await TableComponent.Reload();
            snackbar.Add("Entry added", Severity.Success);
            logger.LogDebug("An entry was successfully added on table '{table}'", Table.DisplayName);
            eventEmitter.PublishEvent(EventType.EntityCreated, entry, Table, TokenSource.Token);
        }
        catch (Exception e) {
            logger.LogError(e, "An error occured while trying to add an entry to the table '{table}'", Table.DisplayName);
            snackbar.Add($"An error occured: {e.Message}", Severity.Error);
        }
    }

    private async Task OnEdit(object entry) {
        logger.LogDebug("Received call for entry editing on table '{table}'", Table.DisplayName);
        var newEntry = await EditorComponent.Present(entry);
        if (newEntry is null) return;
        
        var authorized = await authProvider.IsAuthenticated(Table.EditClaim, TokenSource.Token);
        if (!authorized)
            return;
        
        logger.LogDebug("User is authorized to edit an entry on table '{table}'", Table.DisplayName);
        
        try {
            await Repository.UpdateGenericAsync(newEntry, TokenSource.Token);
            await TableComponent.Reload();
            snackbar.Add("Entry updated", Severity.Success);
            logger.LogDebug("An entry was successfully edited on table '{table}'", Table.DisplayName);
            eventEmitter.PublishEvent(EventType.EntityUpdated, entry, Table, TokenSource.Token);
        }
        catch (Exception e) {
            logger.LogError(e, "An error occured while trying to edit an entry on the table '{table}'", Table.DisplayName);
            snackbar.Add($"An error occured: {e.Message}", Severity.Error);
        }
    }

    private async Task OnDelete(object entry) {
        logger.LogDebug("Received call for entry deleting on table '{table}'", Table.DisplayName);
        var dialog = await dialogs.ShowAsync<DeleteConfirmationDialog>();
        var result = await dialog.Result;

        if (result is not null && !result.Canceled) {
            var authorized = await authProvider.IsAuthenticated(Table.EditClaim, TokenSource.Token);
            if (!authorized)
                return;
            
            logger.LogDebug("User is authorized to delete an entry on table '{table}'", Table.DisplayName);
            
            try {
                await Repository.DeleteGenericAsync(entry, TokenSource.Token);
                await TableComponent.Reload();
                snackbar.Add("Entry deleted", Severity.Success);
                logger.LogDebug("An entry was successfully deleted on table '{table}'", Table.DisplayName);
                eventEmitter.PublishEvent(EventType.EntityDeleted, entry, Table, TokenSource.Token);
            }
            catch (Exception e) {
                logger.LogError(e, "An error occured while trying to delete an entry on the table '{table}'", Table.DisplayName);
                snackbar.Add($"An error occured: {e.Message}", Severity.Error);
            }
        }
    }
}