using System.Collections;
using HopFrame.Core.Configuration;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HopFrame.Web.Components.Components;

public partial class PropertyInput(IEntityAccessor entityAccessor, IDialogService dialogs, IConfigAccessor configAccessor) : ComponentBase {
    [Parameter]
    public object? Value { get; set; }

    [Parameter]
    public required PropertyConfig Config { get; set; }

    [Parameter]
    public Variant Variant { get; set; }

    [Parameter]
    public EventCallback<object?> ValueChanged { get; set; }

    [Parameter]
    public string? Error { get; set; }

    private DateTime _date;
    private TimeSpan _time;
    private string? _relationDisplay;
    private string[] _relationListDisplay = null!;
    private List<string?> _selectedEnums = [];

    private InputType _passwordInputType = InputType.Password;
    private string _passwordIcon = Icons.Material.Filled.VisibilityOff;

    protected override void OnParametersSet() {
        if (Value is null) {
            _date = default;
            _time = TimeSpan.Zero;
            _relationDisplay = null;
            _relationListDisplay = [];
            _selectedEnums = [];
            return;
        }

        if (Config.PropertyType.HasFlag(PropertyType.Relation)) {
            if (Config.PropertyType.HasFlag(PropertyType.List)) {
                _relationListDisplay = ((IEnumerable<object>)Value)
                    .Select(v => entityAccessor.FormatValue(v, Config, true) ?? "")
                    .ToArray();
            }
            else {
                _relationDisplay = entityAccessor.FormatValue(Value, Config);
            }

            return;
        }

        switch ((PropertyType)((byte)Config.PropertyType & 0x0F)) {
            case PropertyType.DateTime:
                _date = (DateTime)Value;
                _time = TimeOnly.FromDateTime((DateTime)Value).ToTimeSpan();
                break;

            case PropertyType.DateOnly:
                _date = ((DateOnly)Value).ToDateTime(TimeOnly.MinValue);
                break;

            case PropertyType.TimeOnly:
                _time = ((TimeOnly)Value).ToTimeSpan();
                break;
        }

        if ((PropertyType)((byte)Config.PropertyType & 0x0F) == PropertyType.Enum && (Config.PropertyType & PropertyType.List) != 0 && Value is not null) {
            var list = (Value as IList)!;
            _selectedEnums.Clear();
            
            foreach (var entry in list) {
                _selectedEnums.Add(entityAccessor.FormatValue(entry, Config, true));
            }
        }
    }

    private async Task OnDateChanged(DateTime? date) {
        if (date is null) return;

        _date = date.Value;
        await OnValueChanged(date.Value);
    }

    private async Task OnTimeChanged(TimeSpan? time) {
        if (time is null) return;

        _time = time.Value;
        await OnValueChanged(time.Value);
    }

    private async Task OnValueChanged(object? value) {
        if (value is DateTime dt) {
            switch ((PropertyType)((byte)Config.PropertyType & 0x0F)) {
                case PropertyType.DateOnly:
                    value = DateOnly.FromDateTime(dt);
                    break;

                case PropertyType.DateTime:
                    value = DateOnly.FromDateTime(dt).ToDateTime(TimeOnly.FromTimeSpan(_time));
                    break;
            }
        }

        if (value is TimeSpan ts) {
            switch ((PropertyType)((byte)Config.PropertyType & 0x0F)) {
                case PropertyType.TimeOnly:
                    value = TimeOnly.FromTimeSpan(ts);
                    break;

                case PropertyType.DateTime:
                    value = DateOnly.FromDateTime(_date).ToDateTime(TimeOnly.FromTimeSpan(ts));
                    break;
            }
        }

        if ((PropertyType)((byte)Config.PropertyType & 0x0F) == PropertyType.Enum && (Config.PropertyType & PropertyType.List) != 0 && value is not null) {
            var list = value as IReadOnlyCollection<object?>;
            var newValue = Activator.CreateInstance(Config.Type) as IList;
            foreach (var entry in list!) {
                newValue!.Add(entry);
            }

            value = newValue;
        }
        else if ((PropertyType)((byte)Config.PropertyType & 0x0F) == PropertyType.Enum && value is not null && !value.GetType().IsEnum)
            return;

        if (value is double d) {
            value = Convert.ChangeType(d, Config.Type);
        }

        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }

    private void OnPasswordIconClick() {
        _passwordIcon = _passwordInputType == InputType.Password ? Icons.Material.Filled.VisibilityOff : Icons.Material.Filled.Visibility;
        _passwordInputType = _passwordInputType == InputType.Password ? InputType.Text : InputType.Password;
    }

    private async Task OnRelationClick() {
        var relationTable = configAccessor.GetTableByType(Config.RelationType!);
        var preselected = new List<object>();

        if (Value is not null) {
            if ((Config.PropertyType & PropertyType.List) != 0) {
                foreach (var entry in (Value as IList)!) {
                    preselected.Add(entry);
                }
            }
            else {
                preselected.Add(Value);
            }
        }

        var parameters = new DialogParameters<RelationPicker> {
            { x => x.Config, relationTable },
            { x => x.Multiple, (Config.PropertyType & PropertyType.List) != 0 },
            { x => x.Preselected, preselected }
        };

        var dialog = await dialogs.ShowAsync<RelationPicker>(string.Empty, parameters, new());
        var result = await dialog.Result;

        if (result is null || result.Canceled || !ValueChanged.HasDelegate)
            return;

        var data = (List<object>)result.Data!;

        if ((Config.PropertyType & PropertyType.List) != 0) {
            var value = (Activator.CreateInstance(Config.Type) as IList)!;

            foreach (var entry in data) {
                value.Add(entry);
            }

            await ValueChanged.InvokeAsync(value);
        }
        else {
            await ValueChanged.InvokeAsync(data.FirstOrDefault());
        }
    }
}