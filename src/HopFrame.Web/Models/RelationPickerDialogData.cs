using HopFrame.Core.Config;

namespace HopFrame.Web.Models;

public class RelationPickerDialogData(TableConfig sourceTable, object? current) {
    public object? Object { get; set; } = current;
    public TableConfig SourceTable { get; init; } = sourceTable;
}