using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Members;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal sealed class AdminPropertyGenerator<TProperty, TModel>(string name, Type type) : IAdminPropertyGenerator<TProperty, TModel>, IGenerator<AdminPageProperty> {
    
    private readonly AdminPageProperty _property = new() {
        Name = name,
        Type = type
    };

    public IAdminPropertyGenerator<TProperty, TModel> Sortable(bool sortable) {
        _property.Sortable = sortable;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Editable(bool editable) {
        _property.Editable = editable;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> DisplayValueWhileEditing(bool display) {
        _property.EditDisplayValue = display;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> DisplayInListing(bool display = true) {
        _property.DisplayInListing = display;
        _property.Sortable = false;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Ignore(bool ignore = false) {
        _property.Ignore = ignore;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Generated(bool generated = true) {
        _property.Generated = generated;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Bold(bool bold = true) {
        _property.Bold = bold;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Unique(bool unique = true) {
        _property.Unique = unique;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> DisplayName(string displayName) {
        _property.DisplayName = displayName;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Prefix(string prefix) {
        _property.Prefix = prefix;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Validator(Func<TProperty, string> validator) {
        _property.Validator = o => validator.Invoke((TProperty)o);
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> IsSelector(bool selector = true) {
        _property.Selector = selector;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> IsSelector<TSelectorType>(bool selector = true) {
        _property.Selector = true;
        _property.SelectorType = typeof(TSelectorType);
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Parser(Func<TModel, string, TProperty> parser) {
        _property.Parser = (o, s) => parser.Invoke((TModel)o, s.ToString());
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Parser<TInput>(Func<TModel, TInput, TProperty> parser) {
        _property.Parser = (o, s) => parser.Invoke((TModel)o, (TInput)s);
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> Parser<TInput, TInnerProperty>(Func<TModel, TInput, TInnerProperty> parser) {
        _property.Parser = (o, s) => parser.Invoke((TModel)o, (TInput)s);
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> DisplayProperty(Expression<Func<TProperty, object>> propertyExpression) {
        var property = AdminPageGenerator<object>.GetPropertyInfo(propertyExpression);
        _property.DisplayPropertyName = property.Name;
        return this;
    }

    public IAdminPropertyGenerator<TProperty, TModel> DisplayProperty<TInnerProperty>(Expression<Func<TInnerProperty, object>> propertyExpression) {
        var property = AdminPageGenerator<object>.GetPropertyInfo(propertyExpression);
        _property.DisplayPropertyName = property.Name;
        return this;
    }

    public AdminPageProperty Compile() {
        _property.DisplayName ??= _property.Name;
        return _property;
    }
    
    public void ApplyConfigurationFromAttributes<T>(AdminPageGenerator<T> pageGenerator, object[] attributes, PropertyInfo property) {
        if (attributes.Any(a => a is KeyAttribute)) {
            pageGenerator.Page.DefaultSortPropertyName = property.Name;
            Editable(false);
            Bold();
        }

        if (attributes.Any(a => a is AdminUnsortableAttribute))
            Sortable(false);

        if (attributes.Any(a => a is AdminUneditableAttribute))
            Editable(false);

        if (attributes.Any(a => a is AdminUniqueAttribute))
            Unique();
        
        if (attributes.Any(a => a is AdminIgnoreAttribute)) {
            var attribute = attributes.Single(a => a is AdminIgnoreAttribute) as AdminIgnoreAttribute;
            DisplayInListing(false);
            Sortable(false);
            Ignore(attribute?.OnlyForListing == false);
        }

        if (attributes.Any(a => a is AdminHideValueAttribute))
            DisplayValueWhileEditing(false);

        if (attributes.Any(a => a is DatabaseGeneratedAttribute))
            Generated();

        if (attributes.Any(a => a is AdminNameAttribute)) {
            var attribute = attributes.Single(a => a is AdminNameAttribute) as AdminNameAttribute;
            DisplayName(attribute?.Name);
        }
        
        if (attributes.Any(a => a is AdminBoldAttribute)) {
            var attribute = attributes.Single(a => a is AdminBoldAttribute) as AdminBoldAttribute;
            Bold(attribute?.Bold == true);
        }

        if (attributes.Any(a => a is RequiredAttribute)) {
            _property.Required = true;
        }

        if (attributes.Any(a => a is AdminPrefixAttribute)) {
            var attribute = attributes.Single(a => a is AdminPrefixAttribute) as AdminPrefixAttribute;
            Prefix(attribute?.Prefix);
        }

        if (attributes.Any(a => a is ListingPropertyAttribute)) {
            var attribute = attributes.Single(a => a is ListingPropertyAttribute) as ListingPropertyAttribute;
            _property.DisplayPropertyName = property.Name;
        }
    }
}