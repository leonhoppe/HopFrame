using HopFrame.Core.Config;

namespace HopFrame.Web.Models;

public sealed class RelationPickerDialogData(TableConfig sourceTable, List<object> current, bool multiple) {
    public List<object> SelectedObjects { get; set; } = current;
    public TableConfig SourceTable { get; init; } = sourceTable;
    public bool AllowMultiple { get; set; } = multiple;
}