using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// Raised before an entry should be deleted
/// </summary>
public sealed class DeleteEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    /// <summary>
    /// The entry that is about to be deleted
    /// </summary>
    public required object Entity { get; init; }
}

/// <summary>
/// Raised before a new entry gets saved to the dataset
/// </summary>
public sealed class CreateEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender);

/// <summary>
/// Raised before any changes are saved to the dataset
/// </summary>
public sealed class UpdateEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    /// <summary>
    /// The entry that is about to be updated
    /// </summary>
    public required object Entity { get; init; }
}

/// <summary>
/// Raised when an entity is selected or deselected in the relation picker dialog
/// </summary>
public sealed class SelectEntryEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender) {
    /// <summary>
    /// The entry that is being selected
    /// </summary>
    public required object Entity { get; init; }
    
    /// <summary>
    /// Indicates whether the entry is selected (true) or deselected (false)
    /// </summary>
    public required bool Selected { get; set; }
}
