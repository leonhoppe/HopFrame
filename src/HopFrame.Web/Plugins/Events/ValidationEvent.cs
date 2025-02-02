using HopFrame.Core.Config;
using HopFrame.Web.Components.Dialogs;

namespace HopFrame.Web.Plugins.Events;

public sealed class ValidationEvent(HopFrameEditor sender) : HopFrameEditorEventArgs(sender) {
    public required IList<string> Errors { get; init; }
    public required PropertyConfig Property { get; init; }
}