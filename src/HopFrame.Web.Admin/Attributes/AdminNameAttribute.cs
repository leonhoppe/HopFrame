namespace HopFrame.Web.Admin.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
public class AdminNameAttribute(string name) : Attribute {
    public string Name { get; set; } = name;
}