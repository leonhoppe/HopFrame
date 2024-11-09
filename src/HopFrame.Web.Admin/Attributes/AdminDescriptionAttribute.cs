namespace HopFrame.Web.Admin.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public sealed class AdminDescriptionAttribute(string description) : Attribute {
    public string Description { get; set; } = description;
}