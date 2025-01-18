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

/// <summary>
/// A helper class for editing the <see cref="PropertyConfig"/>
/// </summary>
public class PropertyConfigurator<TProp>(PropertyConfig config) {
    
    /// <summary>
    /// The Internal property configuration that's modified by the helper functions
    /// </summary>
    public PropertyConfig InnerConfig { get; } = config;

    /// <summary>
    /// Sets the title displayed in the table header and edit dialog
    /// </summary>
    public PropertyConfigurator<TProp> SetDisplayName(string displayName) {
        InnerConfig.Name = displayName;
        return this;
    }

    /// <summary>
    /// Determines if the property should appear in the table, if not the property is also set to be not searchable
    /// </summary>
    /// <seealso cref="IsSearchable"/>
    public PropertyConfigurator<TProp> List(bool list) {
        InnerConfig.List = list;
        InnerConfig.Searchable = !list;
        return this;
    }

    /// <summary>
    /// Determines if the table can be sorted by the property
    /// </summary>
    public PropertyConfigurator<TProp> IsSortable(bool sortable) {
        InnerConfig.Sortable = sortable;
        return this;
    }

    /// <summary>
    /// Determines if the property get taken into account for search results
    /// </summary>
    public PropertyConfigurator<TProp> IsSearchable(bool searchable) {
        InnerConfig.Searchable = searchable;
        return this;
    }

    /// <summary>
    /// Determines if the value that should be displayed instead of the string representation of the type
    /// </summary>
    /// <seealso cref="Format"/>
    public PropertyConfigurator<TProp> SetDisplayedProperty<TInnerProp>(Expression<Func<TProp, TInnerProp>> propertyExpression) {
        InnerConfig.DisplayedProperty = TableConfigurator<TProp>.GetPropertyInfo(propertyExpression);
        return this;
    }

    /// <summary>
    /// Determines the value that's displayed in the admin ui
    /// </summary>
    /// <seealso cref="FormatEach{TInnerProp}"/>
    /// <seealso cref="SetDisplayedProperty{TInnerProp}"/>
    public PropertyConfigurator<TProp> Format(Func<TProp, IServiceProvider, string> formatter) {
        InnerConfig.Formatter = (obj, provider) => formatter.Invoke((TProp)obj, provider);
        return this;
    }

    /// <summary>
    /// Determines the value that's displayed for each entry in the list
    /// </summary>
    public PropertyConfigurator<TProp> FormatEach<TInnerProp>(Func<TInnerProp, IServiceProvider, string> formatter) {
        InnerConfig.EnumerableFormatter = (obj, provider) => formatter.Invoke((TInnerProp)obj, provider);
        return this;
    }

    /// <summary>
    /// Determines the function used for parsing the value provided in the editor dialog to the actual property value
    /// </summary>
    public PropertyConfigurator<TProp> SetParser(Func<string, IServiceProvider, TProp> parser) {
        InnerConfig.Parser = (str, provider) => parser.Invoke(str, provider)!;
        return this;
    }

    /// <summary>
    /// Determines if the value can be edited in the admin ui. If true, the value can still be initially set, but not modified
    /// </summary>
    /// <seealso cref="SetCreatable"/>
    public PropertyConfigurator<TProp> SetEditable(bool editable) {
        InnerConfig.Editable = editable;
        return this;
    }

    /// <summary>
    /// Determines if the initial value can be edited in the admin ui. If true the value will not be visible in the create dialog
    /// </summary>
    /// <seealso cref="SetEditable"/>
    public PropertyConfigurator<TProp> SetCreatable(bool creatable) {
        InnerConfig.Creatable = creatable;
        return this;
    }

    /// <summary>
    /// Determines if the value should be displayed in the admin ui (useful for secrets like passwords etc.)
    /// </summary>
    public PropertyConfigurator<TProp> DisplayValue(bool display) {
        InnerConfig.DisplayValue = display;
        return this;
    }

    /// <summary>
    /// Determines if the admin ui should use a text area for modifying the value
    /// </summary>
    /// <seealso cref="SetTextAreaRows"/>
    public PropertyConfigurator<TProp> IsTextArea(bool textField) {
        InnerConfig.TextArea = textField;
        return this;
    }

    /// <summary>
    /// Determines the initial size of the text area field
    /// </summary>
    /// <seealso cref="IsTextArea"/>
    public PropertyConfigurator<TProp> SetTextAreaRows(int rows) {
        InnerConfig.TextAreaRows = rows;
        return this;
    }

    /// <summary>
    /// Determines the validator used for the property value before saving
    /// </summary>
    public PropertyConfigurator<TProp> SetValidator(Func<TProp?, IServiceProvider, IEnumerable<string>> validator) {
        InnerConfig.Validator = (obj, provider) => Task.FromResult(validator.Invoke((TProp?)obj, provider));
        return this;
    }
    
    /// <summary>
    /// Determines the validator used for the property value before saving
    /// </summary>
    public PropertyConfigurator<TProp> SetValidator(Func<TProp?, IServiceProvider, Task<IEnumerable<string>>> validator) {
        InnerConfig.Validator = (obj, provider) => validator.Invoke((TProp?)obj, provider);
        return this;
    }

    /// <summary>
    /// Determines the order index for the property in the admin ui
    /// </summary>
    /// <seealso cref="TableConfigurator{TModel}.SetOrderIndex"/>
    public PropertyConfigurator<TProp> SetOrderIndex(int index) {
        InnerConfig.Order = index;
        return this;
    }
}
