using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Attributes.Classes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AdminPermissionsAttribute(string view = null, string create = null, string update = null, string delete = null) : Attribute {
    public AdminPagePermissions Permissions { get; set; } = new() {
        Create = create,
        Update = update,
        Delete = delete,
        View = view
    };
}
