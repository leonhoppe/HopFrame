using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

public sealed class DeleteEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public required object Entity { get; init; }
}

public sealed class CreateEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender);

public sealed class UpdateEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public required object Entity { get; init; }
}

public sealed class SelectEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public required object Entity { get; init; }
    public required bool Selected { get; set; }
}
