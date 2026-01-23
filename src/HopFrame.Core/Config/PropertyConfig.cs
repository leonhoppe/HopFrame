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
    public Func<object, IServiceProvider, Task<string>>? Formatter { get; set; }
    public Func<object, IServiceProvider, Task<string>>? EnumerableFormatter { get; set; }
    public Func<string, IServiceProvider, Task<object>>? Parser { get; set; }
    public Func<object?, IServiceProvider, Task<IEnumerable<string>>>? Validator { get; set; }
    public bool Editable { get; set; } = true;
    public bool Creatable { get; set; } = true;
    public bool DisplayValue { get; set; } = true;
    public bool TextArea { get; set; }
    public int TextAreaRows { get; set; } = 16;
    public bool IsRelation { get; internal set; }
    public bool IsRequired { get; internal set; }
    public bool IsEnumerable { get; internal set; }
    public bool IsVirtualProperty { get; set; }
    public int Order { get; set; } = nthProperty;
    public int DisplayLength { get; set; } = 32;

    public virtual object? GetValue(object? source, IServiceProvider provider) {
        return Info.GetValue(source);
    }

    public virtual void SetValue(object? source, object? value, IServiceProvider provider) {
        Info.SetValue(source, value);
    }
}

/// <inheritdoc />
public sealed class VirtualPropertyConfig(TableConfig table, int nthProperty) : PropertyConfig(GetDummyProperty(), table, nthProperty) {

    public string? DummyProperty { get; set; } = null;

    public Func<object, string, IServiceProvider, Task>? VirtualParser { get; set; }

    public override object? GetValue(object? source, IServiceProvider provider) {
        return Formatter!.Invoke(source!, provider).Result;
    }

    public override void SetValue(object? source, object? value, IServiceProvider provider) {
        VirtualParser?.Invoke(source!, (string)value!, provider).Wait();
    }

    private static PropertyInfo GetDummyProperty() {
        return typeof(VirtualPropertyConfig)
            .GetProperties()
            .First(prop => prop.Name == nameof(DummyProperty));
    }
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
    /// <param name="displayName">The new name of the property</param>
    public PropertyConfigurator<TProp> SetDisplayName(string displayName) {
        InnerConfig.Name = displayName;
        return this;
    }

    /// <summary>
    /// Determines if the property should appear in the table, if not the property is also set to be not searchable
    /// </summary>
    /// <param name="list">The toggle for the option</param>
    /// <seealso cref="IsSearchable"/>
    public PropertyConfigurator<TProp> List(bool list) {
        InnerConfig.List = list;
        InnerConfig.Searchable = !list;
        return this;
    }

    /// <summary>
    /// Determines if the table can be sorted by the property
    /// </summary>
    /// <param name="sortable">The toggle for the option</param>
    public PropertyConfigurator<TProp> IsSortable(bool sortable) {
        InnerConfig.Sortable = sortable;
        return this;
    }

    /// <summary>
    /// Determines if the property get taken into account for search results
    /// </summary>
    /// <param name="searchable">The toggle for the option</param>
    public PropertyConfigurator<TProp> IsSearchable(bool searchable) {
        InnerConfig.Searchable = searchable;
        return this;
    }

    /// <summary>
    /// Determines if the value that should be displayed instead of the string representation of the type
    /// </summary>
    /// <param name="propertyExpression">The expression that points at the property that should be used</param>
    public PropertyConfigurator<TProp> SetDisplayedProperty<TInnerProp>(Expression<Func<TProp, TInnerProp>> propertyExpression) {
        InnerConfig.DisplayedProperty = TableConfigurator<TProp>.GetPropertyInfo(propertyExpression);
        return this;
    }

    /// <summary>
    /// Determines the value that's displayed in the admin ui
    /// </summary>
    /// <param name="formatter">The function that formats the given entity</param>
    /// <seealso cref="SetDisplayedProperty{TInnerProp}"/>
    public PropertyConfigurator<TProp> Format(Func<TProp, IServiceProvider, string> formatter) {
        InnerConfig.Formatter = (obj, provider) => Task.FromResult(formatter.Invoke((TProp)obj, provider));
        return this;
    }
    
    /// <inheritdoc cref="Format(System.Func{TProp,System.IServiceProvider,string})"/>
    public PropertyConfigurator<TProp> Format(Func<TProp, IServiceProvider, Task<string>> formatter) {
        InnerConfig.Formatter = (obj, provider) => formatter.Invoke((TProp)obj, provider);
        return this;
    }

    /// <summary>
    /// Determines the value that's displayed for each entry in the list
    /// </summary>
    /// <param name="formatter">The function that formats each given element</param>
    public PropertyConfigurator<TProp> FormatEach<TInnerProp>(Func<TInnerProp, IServiceProvider, string> formatter) {
        InnerConfig.EnumerableFormatter = (obj, provider) => Task.FromResult(formatter.Invoke((TInnerProp)obj, provider));
        return this;
    }
    
    /// <inheritdoc cref="FormatEach{TInnerProp}(System.Func{TInnerProp,System.IServiceProvider,string})"/>
    public PropertyConfigurator<TProp> FormatEach<TInnerProp>(Func<TInnerProp, IServiceProvider, Task<string>> formatter) {
        InnerConfig.EnumerableFormatter = (obj, provider) => formatter.Invoke((TInnerProp)obj, provider);
        return this;
    }

    /// <summary>
    /// Determines the function used for parsing the value provided in the editor dialog to the actual property value
    /// </summary>
    /// <param name="parser">The function that converts the user input to the desired type</param>
    public PropertyConfigurator<TProp> SetParser(Func<string, IServiceProvider, TProp> parser) {
        InnerConfig.Parser = (str, provider) => Task.FromResult<object>(parser.Invoke(str, provider)!);
        return this;
    }
    
    /// <inheritdoc cref="SetParser(System.Func{string,System.IServiceProvider,TProp})"/>
    public PropertyConfigurator<TProp> SetParser(Func<string, IServiceProvider, Task<TProp>> parser) {
        InnerConfig.Parser = async (str, provider) => (await parser.Invoke(str, provider))!;
        return this;
    }

    /// <summary>
    /// Determines if the value can be edited in the admin ui. If true, the value can still be initially set, but not modified
    /// </summary>
    /// <param name="editable">The toggle for the option</param>
    /// <seealso cref="SetCreatable"/>
    public PropertyConfigurator<TProp> SetEditable(bool editable) {
        InnerConfig.Editable = editable;
        return this;
    }

    /// <summary>
    /// Determines if the initial value can be edited in the admin ui. If true the value will not be visible in the create dialog
    /// </summary>
    /// <param name="creatable">The toggle for the option</param>
    /// <seealso cref="SetEditable"/>
    public PropertyConfigurator<TProp> SetCreatable(bool creatable) {
        InnerConfig.Creatable = creatable;
        return this;
    }

    /// <summary>
    /// Determines if the value should be displayed in the admin ui (useful for secrets like passwords etc.)
    /// </summary>
    /// <param name="display">The toggle for the option</param>
    public PropertyConfigurator<TProp> DisplayValue(bool display) {
        InnerConfig.DisplayValue = display;
        return this;
    }

    /// <summary>
    /// Determines if the admin ui should use a text area for modifying the value
    /// </summary>
    /// <param name="textField">The toggle for the option</param>
    /// <seealso cref="SetTextAreaRows"/>
    public PropertyConfigurator<TProp> IsTextArea(bool textField) {
        InnerConfig.TextArea = textField;
        return this;
    }

    /// <summary>
    /// Determines the initial size of the text area field
    /// </summary>
    /// <param name="rows">The number of rows (height) the text area field should have</param>
    /// <seealso cref="IsTextArea"/>
    public PropertyConfigurator<TProp> SetTextAreaRows(int rows) {
        InnerConfig.TextAreaRows = rows;
        return this;
    }

    /// <summary>
    /// Determines the validator used for the property value before saving
    /// </summary>
    /// <param name="validator">
    /// The function that validates the given input.
    ///
    /// It takes in the parsed property and an <see cref="IServiceProvider"/>
    /// and returns an error list. If the list is empty, the property passes the check.
    /// </param>
    public PropertyConfigurator<TProp> SetValidator(Func<TProp?, IServiceProvider, IEnumerable<string>> validator) {
        InnerConfig.Validator = (obj, provider) => Task.FromResult(validator.Invoke((TProp?)obj, provider));
        return this;
    }
    
    /// <inheritdoc cref="SetValidator(System.Func{TProp?,System.IServiceProvider,System.Collections.Generic.IEnumerable{string}})"/>
    public PropertyConfigurator<TProp> SetValidator(Func<TProp?, IServiceProvider, Task<IEnumerable<string>>> validator) {
        InnerConfig.Validator = (obj, provider) => validator.Invoke((TProp?)obj, provider);
        return this;
    }

    /// <summary>
    /// Determines the order index for the property in the admin ui
    /// </summary>
    /// <param name="index">The value for the option</param>
    /// <seealso cref="TableConfigurator{TModel}.SetOrderIndex"/>
    public PropertyConfigurator<TProp> SetOrderIndex(int index) {
        InnerConfig.Order = index;
        return this;
    }

    /// <summary>
    /// Sets the maximum character length displayed in the admin ui (not in the editor dialog)
    /// </summary>
    /// <param name="maxLength">The maximum length of characters to be displayed</param>
    public PropertyConfigurator<TProp> SetDisplayLength(int maxLength) {
        InnerConfig.DisplayLength = maxLength;
        return this;
    }

    /// <summary>
    /// Forces a property to be treated as a relation
    /// </summary>
    /// <param name="isEnumerable">Determines if it is possible to assign multiple objects to the property</param>
    /// <param name="isRequired">Determines if the property is nullable</param>
    public PropertyConfigurator<TProp> ForceRelation(bool isEnumerable = false, bool isRequired = true) {
        InnerConfig.IsRelation = true;
        InnerConfig.IsEnumerable = isEnumerable;
        InnerConfig.IsRequired = isRequired;
        return this;
    }
}

/// <inheritdoc/>
public sealed class VirtualPropertyConfigurator<TModel>(VirtualPropertyConfig config) : PropertyConfigurator<string>(config) {
    /// <summary>
    /// Determines the function used for parsing the value provided in the editor dialog to the actual model value
    /// </summary>
    /// <param name="parser">The function that takes in the parent object and the user input and applies all necessary changes</param>
    public VirtualPropertyConfigurator<TModel> SetVirtualParser(Action<TModel, string, IServiceProvider> parser) {
        var cfg = InnerConfig as VirtualPropertyConfig;

        cfg!.VirtualParser = (model, input, services) => {
            parser.Invoke((TModel)model, input, services);
            return Task.CompletedTask;
        };
        
        return this;
    }

    /// <inheritdoc cref="SetVirtualParser(System.Action{TModel, System.String, System.IServiceProvider})" />
    public VirtualPropertyConfigurator<TModel> SetVirtualParser(Func<TModel, string, IServiceProvider, Task> parser) {
        var cfg = InnerConfig as VirtualPropertyConfig;
        
        cfg!.VirtualParser = (model, input, services) => parser.Invoke((TModel)model, input, services);
        
        return this;
    }
}
