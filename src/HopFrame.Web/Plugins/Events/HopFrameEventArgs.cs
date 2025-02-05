using HopFrame.Core.Config;
using HopFrame.Web.Components.Dialogs;
using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

public abstract class HopFrameEventArgs(object internalSender) {
    internal object InternalSender { get; } = internalSender;
    public bool IsCanceled { get; protected set; }
    
    
    public void SetCancelled(bool canceled) => IsCanceled = canceled;
}

public abstract class HopFrameEventArgs<TSender>(TSender sender) : HopFrameEventArgs(sender) where TSender : class {
    public TSender Sender => (TSender)InternalSender;
}

public abstract class HopFrameTablePageEventArgs(HopFrameTablePage sender)
    : HopFrameEventArgs<HopFrameTablePage>(sender) {
    public required TableConfig Table { get; init; }
}

public abstract class HopFrameEditorEventArgs(HopFrameEditor sender)
    : HopFrameEventArgs<HopFrameEditor>(sender) {
    public required TableConfig Table { get; init; }
}
