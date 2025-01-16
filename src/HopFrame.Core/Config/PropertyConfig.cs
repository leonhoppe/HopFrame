using System.Linq.Expressions;
using System.Reflection;

namespace HopFrame.Core.Config;

public class PropertyConfig(PropertyInfo info, TableConfig table) {
    public PropertyInfo Info { get; } = info;
    public TableConfig Table { get; } = table;
    public string Name { get; set; } = info.Name;
    public bool List { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public bool Searchable { get; set; } = true;
    public PropertyInfo? DisplayedProperty { get; set; }
    public Func<object, string>? Formatter { get; set; }
    public Func<string, object>? Parser { get; set; }
    public Func<object?, Task<IEnumerable<string>>>? Validator { get; set; }
    public bool Editable { get; set; } = true;
    public bool Creatable { get; set; } = true;
    public bool DisplayValue { get; set; } = true;
    public bool IsRelation { get; set; }
    public bool IsRequired { get; set; }
}

public class PropertyConfig<TProp>(PropertyConfig config) {
    public PropertyConfig InnerConfig { get; } = config;

    public PropertyConfig<TProp> SetDisplayName(string displayName) {
        InnerConfig.Name = displayName;
        return this;
    }

    public PropertyConfig<TProp> List(bool list) {
        InnerConfig.List = list;
        InnerConfig.Searchable = false;
        return this;
    }

    public PropertyConfig<TProp> Sortable(bool sortable) {
        InnerConfig.Sortable = sortable;
        return this;
    }

    public PropertyConfig<TProp> Searchable(bool searchable) {
        InnerConfig.Searchable = searchable;
        return this;
    }

    public PropertyConfig<TProp> DisplayedProperty<TInnerProp>(Expression<Func<TProp, TInnerProp>> propertyExpression) {
        InnerConfig.DisplayedProperty = TableConfig<TProp>.GetPropertyInfo(propertyExpression);
        return this;
    }

    public PropertyConfig<TProp> Format(Func<TProp, string> formatter) {
        InnerConfig.Formatter = obj => formatter.Invoke((TProp)obj);
        return this;
    }

    public PropertyConfig<TProp> ValueParser(Func<string, TProp> parser) {
        InnerConfig.Parser = str => parser.Invoke(str)!;
        return this;
    }

    public PropertyConfig<TProp> Editable(bool editable) {
        InnerConfig.Editable = editable;
        return this;
    }

    public PropertyConfig<TProp> Creatable(bool creatable) {
        InnerConfig.Creatable = creatable;
        return this;
    }

    public PropertyConfig<TProp> DisplayValue(bool display) {
        InnerConfig.DisplayValue = display;
        return this;
    }

    public PropertyConfig<TProp> Validator(Func<TProp?, IEnumerable<string>> validator) {
        InnerConfig.Validator = obj => Task.FromResult(validator.Invoke((TProp?)obj));
        return this;
    }
    
    public PropertyConfig<TProp> Validator(Func<TProp?, Task<IEnumerable<string>>> validator) {
        InnerConfig.Validator = obj => validator.Invoke((TProp?)obj);
        return this;
    }
}
