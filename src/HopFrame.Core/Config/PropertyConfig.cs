using System.Reflection;

namespace HopFrame.Core.Config;

public class PropertyConfig(PropertyInfo info) {
    public PropertyInfo Info { get; init; } = info;
    public string Name { get; set; } = info.Name;
    public bool List { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public bool Searchable { get; set; } = true;
}

public class PropertyConfig<TProp>(PropertyConfig config) {

    public PropertyConfig<TProp> SetDisplayName(string displayName) {
        config.Name = displayName;
        return this;
    }

    public PropertyConfig<TProp> List(bool display) {
        config.List = display;
        config.Searchable = false;
        return this;
    }

    public PropertyConfig<TProp> Sortable(bool sortable) {
        config.Sortable = sortable;
        return this;
    }

    public PropertyConfig<TProp> Searchable(bool searchable) {
        config.Searchable = searchable;
        return this;
    }
    
}
