namespace HopFrame.Web.Admin.Attributes;

/// <summary>
/// This attribute specifies the url of the admin page and needs to be applied on the AdminPage property in the AdminContext directly
/// </summary>
/// <param name="url">The page url: '/administration/{url}'</param>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AdminPageUrlAttribute(string url) : Attribute {
    public string Url { get; set; } = url;
}