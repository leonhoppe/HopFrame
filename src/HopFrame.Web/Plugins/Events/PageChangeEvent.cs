using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// Raised when the user is about to change the page he is currently on
/// </summary>
public sealed class PageChangeEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    /// <summary>
    /// The page he is currently on
    /// </summary>
    public required int CurrentPage { get; init; }
    
    /// <summary>
    /// The total number of pages his table can currently display
    /// </summary>
    public required int TotalPages { get; init; }
    
    /// <summary>
    /// The new page he tries to access
    /// </summary>
    public required int NewPage { get; set; }
}