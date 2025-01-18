using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace HopFrame.Core.Config;

public class PropertyConfig(PropertyInfo info, TableConfig table, int nthProperty) {
    public PropertyInfo Info { get; } = info;
    public TableConfig Table { get; } = table;
    public string Name { get; set; } = info.Name;
    public bool List { get; set; } = true;
    public bool Sortable { get; set; } = true;
    public bool Searchable { get; set; } = true;
    public PropertyInfo? DisplayedProperty { get; set; }
    public Func<object, IServiceProvider, string>? Formatter { get; set; }
    public Func<object, IServiceProvider, string>? EnumerableFormatter { get; set; }
    public Func<string, IServiceProvider, object>? Parser { get; set; }
    public Func<object?, IServiceProvider, Task<IEnumerable<string>>>? Validator { get; set; }
    public bool Editable { get; set; } = true;
    public bool Creatable { get; set; } = true;
    public bool DisplayValue { get; set; } = true;
    public bool TextArea { get; set; }
    public int TextAreaRows { get; set; } = 16;
    public bool IsRelation { get; set; }
    public bool IsRequired { get; set; }
    public bool IsEnumerable { get; set; }
    public bool IsListingProperty { get; set; }
    public int Order { get; set; } = nthProperty;
}

public class PropertyConfig<TProp>(PropertyConfig config) {
    public PropertyConfig InnerConfig { get; } = config;

    public PropertyConfig<TProp> SetDisplayName(string displayName) {
        InnerConfig.Name = displayName;
        return this;
    }

    public PropertyConfig<TProp> List(bool list) {
        InnerConfig.List = list;
        InnerConfig.Searchable = !list;
        return this;
    }

    public PropertyConfig<TProp> IsSortable(bool sortable) {
        InnerConfig.Sortable = sortable;
        return this;
    }

    public PropertyConfig<TProp> IsSearchable(bool searchable) {
        InnerConfig.Searchable = searchable;
        return this;
    }

    public PropertyConfig<TProp> SetDisplayedProperty<TInnerProp>(Expression<Func<TProp, TInnerProp>> propertyExpression) {
        InnerConfig.DisplayedProperty = TableConfig<TProp>.GetPropertyInfo(propertyExpression);
        return this;
    }

    public PropertyConfig<TProp> Format(Func<TProp, IServiceProvider, string> formatter) {
        InnerConfig.Formatter = (obj, provider) => formatter.Invoke((TProp)obj, provider);
        return this;
    }

    public PropertyConfig<TProp> FormatEach<TInnerProp>(Func<TInnerProp, IServiceProvider, string> formatter) {
        InnerConfig.EnumerableFormatter = (obj, provider) => formatter.Invoke((TInnerProp)obj, provider);
        return this;
    }

    public PropertyConfig<TProp> SetParser(Func<string, IServiceProvider, TProp> parser) {
        InnerConfig.Parser = (str, provider) => parser.Invoke(str, provider)!;
        return this;
    }

    public PropertyConfig<TProp> SetEditable(bool editable) {
        InnerConfig.Editable = editable;
        return this;
    }

    public PropertyConfig<TProp> SetCreatable(bool creatable) {
        InnerConfig.Creatable = creatable;
        return this;
    }

    public PropertyConfig<TProp> DisplayValue(bool display) {
        InnerConfig.DisplayValue = display;
        return this;
    }

    public PropertyConfig<TProp> IsTextArea(bool textField) {
        InnerConfig.TextArea = textField;
        return this;
    }

    public PropertyConfig<TProp> SetTextAreaRows(int rows) {
        InnerConfig.TextAreaRows = rows;
        return this;
    }

    public PropertyConfig<TProp> SetValidator(Func<TProp?, IServiceProvider, IEnumerable<string>> validator) {
        InnerConfig.Validator = (obj, provider) => Task.FromResult(validator.Invoke((TProp?)obj, provider));
        return this;
    }
    
    public PropertyConfig<TProp> SetValidator(Func<TProp?, IServiceProvider, Task<IEnumerable<string>>> validator) {
        InnerConfig.Validator = (obj, provider) => validator.Invoke((TProp?)obj, provider);
        return this;
    }

    public PropertyConfig<TProp> SetOrderIndex(int index) {
        InnerConfig.Order = index;
        return this;
    }
}
