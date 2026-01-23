using HopFrame.Core.Config;
using HopFrame.Web.Components.Dialogs;
using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// The base event with parameters for every event
/// </summary>
public abstract class HopFrameEventArgs(object internalSender) {
    internal object InternalSender { get; } = internalSender;
    
    /// <summary>
    /// Indicates if the event got canceled by any event handler
    /// </summary>
    public bool IsCanceled { get; protected set; }
    
    /// <summary>
    /// Determines if the action that is about to be executed should be canceled
    /// </summary>
    public void SetCancelled(bool canceled) => IsCanceled = canceled;
}

/// <inheritdoc/>
public abstract class HopFrameEventArgs<TSender>(TSender sender) : HopFrameEventArgs(sender) where TSender : class {
    /// <summary>
    /// The element that raised the event
    /// </summary>
    public TSender Sender => (TSender)InternalSender;
}

/// <summary>
/// The base event with parameters for every event raised by a <see cref="HopFrameTablePage"/>
/// </summary>
public abstract class HopFrameTablePageEventArgs(HopFrameTablePage sender) : HopFrameEventArgs<HopFrameTablePage>(sender) {
    /// <summary>
    /// The configuration of the table page that raised the event
    /// </summary>
    public required TableConfig Table { get; init; }
}

/// <summary>
/// The base event with parameters for every event raised by a <see cref="HopFrameEditor"/>
/// </summary>
public abstract class HopFrameEditorEventArgs(HopFrameEditor sender) : HopFrameEventArgs<HopFrameEditor>(sender) {
    /// <summary>
    /// The configuration of the parent table of the editor that raised the event
    /// </summary>
    public required TableConfig Table { get; init; }
}
