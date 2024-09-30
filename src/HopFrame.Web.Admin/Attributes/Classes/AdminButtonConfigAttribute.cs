namespace HopFrame.Web.Admin.Attributes.Classes;

[AttributeUsage(AttributeTargets.Class)]
public class AdminButtonConfigAttribute(bool showCreateButton = true, bool showDeleteButton = true, bool showUpdateButton = true) : Attribute {
    public bool ShowCreateButton { get; set; } = showCreateButton;
    public bool ShowDeleteButton { get; set; } = showDeleteButton;
    public bool ShowUpdateButton { get; set; } = showUpdateButton;
}