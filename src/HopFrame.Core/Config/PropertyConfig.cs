using System.Linq.Expressions;
using System.Reflection;

namespace HopFrame.Core.Config;

public class PropertyConfig(PropertyInfo info) {
    public PropertyInfo Info { get; init; } = info;
    public string Name { get; set; } = info.Name;
    public bool List { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public bool Searchable { get; set; } = true;
    public PropertyInfo? DisplayedProperty { get; set; }
    public Func<object, string>? Formatter { get; set; }
    public bool Editable { get; set; } = true;
    public bool Creatable { get; set; } = true;
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

    public PropertyConfig<TProp> DisplayedProperty<TInnerProp>(Expression<Func<TProp, TInnerProp>> propertyExpression) {
        config.DisplayedProperty = TableConfig<TProp>.GetPropertyInfo(propertyExpression);
        return this;
    }

    public PropertyConfig<TProp> Format(Func<TProp, string> formatter) {
        config.Formatter = obj => formatter.Invoke((TProp)obj);
        return this;
    }

    public PropertyConfig<TProp> Editable(bool editable) {
        config.Editable = editable;
        return this;
    }

    public PropertyConfig<TProp> Creatable(bool creatable) {
        config.Creatable = creatable;
        return this;
    }
    
}
