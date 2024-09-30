namespace HopFrame.Web.Admin.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class AdminDescriptionAttribute(string description) : Attribute {
    public string Description { get; set; } = description;
}