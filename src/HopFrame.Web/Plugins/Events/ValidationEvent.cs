using HopFrame.Core.Config;
using HopFrame.Web.Components.Dialogs;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// Raised when a property in the editor is validated
/// </summary>
public sealed class ValidationEvent(HopFrameEditor sender) : HopFrameEditorEventArgs(sender) {
    /// <summary>
    /// All errors found by the validators. Can be modified to display more (or less) errors
    /// </summary>
    public required IList<string> Errors { get; init; }
    
    /// <summary>
    /// The property that is validated
    /// </summary>
    public required PropertyConfig Property { get; init; }
}