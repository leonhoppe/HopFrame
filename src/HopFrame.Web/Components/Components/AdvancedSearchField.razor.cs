using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;
using System.ComponentModel;

namespace HopFrame.Web.Components.Components;

public partial class AdvancedSearchField(IConfigAccessor configAccessor, IEntityAccessor entityAccessor, ISnackbar snackbar, ILogger<AdvancedSearchField> logger) : ComponentBase {

    private readonly record struct RelationResult(object Value, string Label);

    [Parameter]
    public required PropertyConfig Property { get; set; }

    [Parameter]
    public required EventCallback<AdvancedSearchProperty?> OnValueChanged { get; set; }

    private string CurrentType { get; set; } = "=";

    private string CurrentTypeIcon => $"""<text x="12" y="14" text-anchor="middle" dominant-baseline="middle">{CurrentType}</text>""";

    private PropertyType CleanedType => Property.PropertyType.HasFlag(PropertyType.List) ? PropertyType.List : (PropertyType)((byte)Property.PropertyType & 0x0F);

    private int MinWidth => CleanedType switch {
        PropertyType.DateOnly or PropertyType.TimeOnly => 180,
        PropertyType.DateTime => 220,
        PropertyType.Numeric => 120,
        _ => 150
    };

    private object? MainValue { get; set; }

    private object? SecondaryValue { get; set; }

    private string? Error { get; set; }

    private string? SecondaryError { get; set; }

    private bool IsExact { get; set; }

    private async Task ToggleCurrentType() {
        var availableTypes = GetAppliableTypes();
        var currIndex = availableTypes.IndexOf(CurrentType);
        SecondaryValue = null;
        if (currIndex == -1) {
            CurrentType = availableTypes[0];
            return;
        }

        var newIndex = (currIndex + 1) % availableTypes.Length;
        CurrentType = availableTypes[newIndex];

        if (MainValue is not null)
            await OnValueChangedInternal();
    }

    private string[] GetAppliableTypes() {
        HashSet<string> types = ["="];

        PropertyType[] numericTypes = [PropertyType.Numeric, PropertyType.DateTime, PropertyType.DateOnly, PropertyType.TimeOnly];

        if (numericTypes.Contains(CleanedType) || Property.PropertyType.HasFlag(PropertyType.List)) {
            types.Add("<");
            types.Add(">");
            types.Add("<>");
        }

        return types.ToArray();
    }

    private async Task OnValueChangedInternal() {
        if (MainValue is null && CurrentType != "<>") {
            await OnValueChanged.InvokeAsync(null);
            return;
        }

        if (CurrentType == "<>") {
            if (MainValue is null && SecondaryValue is null) {
                await OnValueChanged.InvokeAsync(null);
                return;
            }

            if (MainValue is not null && SecondaryValue is not null) {
                await OnValueChanged.InvokeAsync(new() {
                    Identifier = Property.Identifier,
                    LessThan = SecondaryValue,
                    MoreThan = MainValue,
                    Exact = IsExact
                });
                return;
            }
        }

        switch (CurrentType) {
            case "=":
                await OnValueChanged.InvokeAsync(new() {
                    Identifier = Property.Identifier,
                    Equal = MainValue,
                    Exact = IsExact
                });
                return;

            case "<":
                await OnValueChanged.InvokeAsync(new() {
                    Identifier = Property.Identifier,
                    LessThan = MainValue,
                    Exact = IsExact
                });
                return;

            case ">":
                await OnValueChanged.InvokeAsync(new() {
                    Identifier = Property.Identifier,
                    MoreThan = MainValue,
                    Exact = IsExact
                });
                return;
        }
    }

    private async Task OnInputValueChanged(object? value, bool exact = false) {
        Error = null;
        IsExact = exact;

        if (value is null || (value is string text && string.IsNullOrWhiteSpace(text))) {
            MainValue = null;
        }
        else {
            var parsed = Parse(value, out var error);
            Error = error;
            if (parsed is null) return;

            MainValue = parsed;
        }

        await OnValueChangedInternal();
    }

    private async Task OnSecondInputValueChanged(object? value) {
        SecondaryError = null;

        if (value is null || (value is string text && string.IsNullOrWhiteSpace(text))) {
            SecondaryValue = null;
        }
        else {
            var parsed = Parse(value, out var error);
            SecondaryError = error;
            if (parsed is null) return;

            SecondaryValue = parsed;
        }

        await OnValueChangedInternal();
    }

    private object? Parse(object value, out string? error) {
        if (Property.PropertyType.HasFlag(PropertyType.List)) {
            if (!int.TryParse(value.ToString()!, out var index)) {
                error = "Invalid count";
                return null;
            }

            value = index;
        }

        else if (value.GetType() != Property.Type) {
            try {
                if (Property.Type == typeof(Guid)) {
                    value = Guid.Parse(value.ToString()!);
                }

                else if (Property.Type == typeof(DateTime)) {
                    value = DateTime.Parse(value.ToString()!);
                }

                else if (Property.Type == typeof(DateOnly)) {
                    value = DateOnly.Parse(value.ToString()!);
                }

                else if (Property.Type == typeof(TimeOnly)) {
                    value = TimeOnly.Parse(value.ToString()!);
                }

                else {
                    value = Convert.ChangeType(value, Property.Type);
                }
            }
            catch {
                error = "Invalid search term";
                return null;
            }
        }

        error = null;
        return value;
    }

    private async Task<IEnumerable<RelationResult>> SearchInDropdown(string? value, CancellationToken ct) {
        try {
            var _relationTable = configAccessor.GetTableByIdentifier(Property.RelationTable!);
            var repo = configAccessor.LoadRepository(_relationTable!);
            IEnumerable<object> result;

            if (!string.IsNullOrWhiteSpace(value)) {
                var searchResult = await repo.SearchGenericAsync(value, 0, Property.MaxDropdownItems, new(null, ListSortDirection.Ascending), ct);
                result = searchResult.Result;
            }
            else {
                result = await repo.LoadPageGenericAsync(0, Property.MaxDropdownItems, new(null, ListSortDirection.Ascending), ct);
            }

            return result.Select(r => new RelationResult(r, entityAccessor.FormatValue(r, Property) ?? "Something went wrong"));
        }
        catch (Exception e) {
            logger.LogError(e, "An error occured while trying to search through the relation table '{table}'", Property.RelationTable);
            snackbar.Add($"An error occured", Severity.Error);
            return Enumerable.Empty<RelationResult>();
        }
    }

}
