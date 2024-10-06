namespace HopFrame.Web.Admin.Attributes.Classes;

[AttributeUsage(AttributeTargets.Class)]
public class AdminUrlAttribute(string url) : Attribute {
    public string Url { get; set; } = url;
}