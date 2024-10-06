using System.Text.Json.Serialization;

namespace HopFrame.Web.Admin.Models;

public sealed class AdminPageProperty {
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string Prefix { get; set; }

    public bool DisplayInListing { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public bool Editable { get; set; } = true;
    public bool EditDisplayValue { get; set; } = true;
    public bool Generated { get; set; }
    public bool Ignore { get; set; }
    [JsonIgnore]
    public Type Type { get; set; }
}