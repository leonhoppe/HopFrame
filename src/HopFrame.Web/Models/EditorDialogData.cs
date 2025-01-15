using HopFrame.Core.Config;

namespace HopFrame.Web.Models;

public sealed class EditorDialogData(TableConfig config, object? current = null) {
    public object? CurrentObject { get; set; } = current;
    public TableConfig Config { get; } = config;
}
