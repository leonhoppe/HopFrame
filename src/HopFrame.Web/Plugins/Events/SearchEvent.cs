using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

public sealed class SearchEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public required string SearchTerm { get; set; }
}