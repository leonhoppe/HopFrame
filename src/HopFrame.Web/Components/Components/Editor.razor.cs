using HopFrame.Core.Configuration;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HopFrame.Web.Components.Components;

public partial class Editor(IDialogService dialogs) : ComponentBase {
    
    private enum EditorMode {
        Editor,
        Creator
    }

    [Parameter]
    public required TableConfig Config { get; set; }

    private bool IsVisible { get; set; }
    
    private object? Entry { get; set; }
    
    private EditorMode Mode { get; set; }

    private TaskCompletionSource<object?> Completion { get; set; } = null!;

    public Task<object?> Present(object? entry) {
        Completion = new ();
        Mode = entry is null ? EditorMode.Creator : EditorMode.Editor;
        Entry = entry ?? Activator.CreateInstance(Config.TableType);
        StateHasChanged();
        IsVisible = true;
        return Completion.Task;
    }

    private async Task Submit() {
        var dialog = await dialogs.ShowAsync<ModifyConfirmationDialog>();
        var result = await dialog.Result;

        if (result is not null && !result.Canceled) {
            ApplyChanges();
            IsVisible = false;
            Completion.SetResult(Entry);
        }
    }

    private void Cancel() {
        IsVisible = false;
        Completion.SetResult(null);
    }

    private IEnumerable<PropertyConfig> GetProperties() {
        var query = (IEnumerable<PropertyConfig>)Config.Properties;

        if (Mode == EditorMode.Creator)
            query = query.Where(p => p.Creatable);

        return query.OrderBy(p => p.OrderIndex);
    }

    private void ApplyChanges() {
        
    }
    
}