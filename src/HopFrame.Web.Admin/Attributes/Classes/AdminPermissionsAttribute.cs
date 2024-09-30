using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Attributes.Classes;

[AttributeUsage(AttributeTargets.Class)]
public class AdminPermissionsAttribute(AdminPagePermissions permissions) : Attribute {
    public AdminPagePermissions Permissions { get; set; } = permissions;
}
