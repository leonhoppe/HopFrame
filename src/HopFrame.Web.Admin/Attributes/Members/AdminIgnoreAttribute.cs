namespace HopFrame.Web.Admin.Attributes.Members;

[AttributeUsage(AttributeTargets.Property)]
public sealed class AdminIgnoreAttribute(bool onlyForListing = false) : Attribute {
    public bool OnlyForListing { get; set; } = onlyForListing;
}
