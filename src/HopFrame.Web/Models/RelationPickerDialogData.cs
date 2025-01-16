using HopFrame.Core.Config;

namespace HopFrame.Web.Models;

public sealed class RelationPickerDialogData(TableConfig sourceTable, object? current) {
    public object? Object { get; set; } = current;
    public TableConfig SourceTable { get; init; } = sourceTable;
}