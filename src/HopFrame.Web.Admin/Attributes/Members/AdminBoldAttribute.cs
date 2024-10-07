namespace HopFrame.Web.Admin.Attributes.Members;

[AttributeUsage(AttributeTargets.Property)]
public class AdminBoldAttribute(bool bold = true) : Attribute {
    public bool Bold { get; set; } = bold;
}