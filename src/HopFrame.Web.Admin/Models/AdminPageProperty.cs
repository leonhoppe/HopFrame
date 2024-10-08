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
    public bool Bold { get; set; }
    public bool Required { get; set; }
    public bool Ignore { get; set; }
    [JsonIgnore]
    public Type Type { get; set; }

    public Func<object, bool> Validator { get; set; }
    
    public object GetValue(object entry) {
        return entry.GetType().GetProperty(Name)?.GetValue(entry);
    }
    
    public T GetValue<T>(object entry) {
        return (T)entry.GetType().GetProperty(Name)?.GetValue(entry);
    }
    
    public void SetValue(object entry, object value) {
        entry.GetType().GetProperty(Name)?.SetValue(entry, value);
    }
}