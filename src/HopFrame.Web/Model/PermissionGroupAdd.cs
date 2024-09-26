using HopFrame.Database.Models;

namespace HopFrame.Web.Model;

internal sealed class PermissionGroupAdd : PermissionGroup {
    public string GroupName { get; set; }
}