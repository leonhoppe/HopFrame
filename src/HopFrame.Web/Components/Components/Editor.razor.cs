using HopFrame.Core.Configuration;
using HopFrame.Core.Events;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HopFrame.Web.Components.Components;

public partial class Editor(IDialogService dialogs, IEntityAccessor accessor, IEventEmitter events) : CancellableComponent {
    
    private enum EditorMode {
        Editor,
        Creator
    }

    [Parameter]
    public required TableConfig Config { get; set; }

    private bool IsVisible { get; set; }

    private object? Entry { get; set; }
    
    private EditorMode Mode { get; set; }

    private bool IsLoading { get; set; }

    private bool GlobalyLoading { get; set; }

    private TaskCompletionSource<object?> Completion { get; set; } = null!;

    private Dictionary<string, object?> UpdatedValues { get; set; } = null!;

    private Dictionary<string, string?> ErrorMessages { get; set; } = null!;

    public Task<object?> Present(object? entry) {
        IsLoading = true;
        Completion = new ();
        Mode = entry is null ? EditorMode.Creator : EditorMode.Editor;
        UpdatedValues = new();

        ErrorMessages = new();
        foreach (var property in GetProperties()) {
            ErrorMessages.Add(property.Identifier, null);
        }

        if (entry is null) {
            InvokeAsync(async () => {
                Entry = Activator.CreateInstance(Config.TableType)!;
                Entry = await events.PublishCallback<IEntityCreateCallbackHandler>(Entry, Config, TokenSource.Token);
                IsLoading = false;
                StateHasChanged();
            });
        } else {
            Entry = entry;
            IsLoading = false;
        }
        
        IsVisible = true;
        return Completion.Task;
    }

    private async Task Submit() {
        if (!Validate())
            return;
        
        var dialog = await dialogs.ShowAsync<ModifyConfirmationDialog>();
        var result = await dialog.Result;

        if (result is not null && !result.Canceled) {
            GlobalyLoading = true;
            StateHasChanged();
            ApplyChanges();
            IsVisible = false;
            Entry = await events.PublishCallback<IEntityUpdateCallbackHandler>(Entry!, Config, TokenSource.Token);
            Completion.SetResult(Entry);
            GlobalyLoading = false;
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

        return query
            .Where(p => p.VisibleInEditor)
            .OrderBy(p => p.OrderIndex);
    }

    private object? GetPropertyValue(PropertyConfig property) {
        if (UpdatedValues.TryGetValue(property.Identifier, out var value) && value is not null)
            return value;
        
        return accessor.GetValueRaw(Entry!, property);
    }

    private void OnPropertyUpdated(PropertyConfig property, object? value) {
        UpdatedValues[property.Identifier] = value;
        Validate(property);
    }

    private void ApplyChanges() {
        foreach (var propUpdate in UpdatedValues) {
            var property = Config.Properties.First(p => p.Identifier == propUpdate.Key);

            if ((PropertyType)((byte)property.PropertyType & 0x0F) == PropertyType.Password) {
                if (propUpdate.Value is not null && string.IsNullOrWhiteSpace((string)propUpdate.Value))
                    continue;
            }
            
            accessor.SetValue(Entry!, property, propUpdate.Value);
        }
    }

    private bool Validate(PropertyConfig? property = null) {
        if (property is null) {
            var valid = true;
            foreach (var propertyConfig in GetProperties()) {
                if (!Validate(propertyConfig))
                    valid = false;
            }

            return valid;
        }
        
        if (!UpdatedValues.TryGetValue(property.Identifier, out var value)) {
            value = accessor.GetValueRaw(Entry!, property);
        }

        var errors = accessor.ValidateProperty(property, value).ToArray();
        ErrorMessages[property.Identifier] = errors.FirstOrDefault();
        return errors.Length == 0;
    }
    
}