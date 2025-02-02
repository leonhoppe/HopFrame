using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

public sealed class PageChangeEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    public required int CurrentPage { get; init; }
    public required int TotalPages { get; init; }
    public required int NewPage { get; set; }
}