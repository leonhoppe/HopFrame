using HopFrame.Web.Components.Pages;

namespace HopFrame.Web.Plugins.Events;

/// <summary>
/// Raised before the table is about to reload its data
/// </summary>
public sealed class ReloadEvent(HopFrameTablePage sender) : HopFrameTablePageEventArgs(sender);