namespace HopFrame.Web.Admin.Attributes.Members;

[AttributeUsage(AttributeTargets.Property)]
public class AdminIgnoreAttribute(bool onlyForListing = false) : Attribute {
    public bool OnlyForListing { get; set; } = onlyForListing;
}
