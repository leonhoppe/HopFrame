namespace HopFrame.Web.Admin.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AdminDescriptionAttribute(string description) : Attribute {
    public string Description { get; set; } = description;
}