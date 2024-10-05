namespace HopFrame.Web.Admin.Attributes.Members;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AdminPrefixAttribute(string prefix) : Attribute {
    public string Prefix { get; set; } = prefix;
}