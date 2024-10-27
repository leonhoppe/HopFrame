using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Members;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal sealed class AdminPropertyGenerator<TProperty>(string name, Type type) : IAdminPropertyGenerator<TProperty>, IGenerator<AdminPageProperty> {
    
    private readonly AdminPageProperty _property = new() {
        Name = name,
        Type = type
    };

    public IAdminPropertyGenerator<TProperty> Sortable(bool sortable) {
        _property.Sortable = sortable;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Editable(bool editable) {
        _property.Editable = editable;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> DisplayValueWhileEditing(bool display) {
        _property.EditDisplayValue = display;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> DisplayInListing(bool display = true) {
        _property.DisplayInListing = display;
        _property.Sortable = false;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Ignore(bool ignore = false) {
        _property.Ignore = ignore;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Generated(bool generated = true) {
        _property.Generated = generated;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Bold(bool bold = true) {
        _property.Bold = bold;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Unique(bool unique = true) {
        _property.Unique = unique;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> DisplayName(string displayName) {
        _property.DisplayName = displayName;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Description(string description) {
        _property.Description = description;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Prefix(string prefix) {
        _property.Prefix = prefix;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Validator(Func<TProperty, string> validator) {
        _property.Validator = o => validator.Invoke((TProperty)o);
        return this;
    }

    public IAdminPropertyGenerator<TProperty> IsSelector<TSelector>() {
        _property.SelectorType = typeof(TSelector);
        return this;
    }

    public IAdminPropertyGenerator<TProperty> Parser<TModel>(Func<TModel, string, TProperty> parser) {
        _property.Parser = (o, s) => parser.Invoke((TModel)o, s);
        return this;
    }

    public IAdminPropertyGenerator<TProperty> ParserForListType<TModel, TInnerProperty>(Func<TModel, string, TInnerProperty> parser) {
        _property.Parser = (o, s) => parser.Invoke((TModel)o, s);
        return this;
    }

    public IAdminPropertyGenerator<TProperty> DisplayProperty<TListingProperty>(Expression<Func<TProperty, TListingProperty>> propertyExpression) {
        var property = AdminPageGenerator<object>.GetPropertyInfo(propertyExpression);
        _property.DisplayPropertyName = property.Name;
        return this;
    }

    public IAdminPropertyGenerator<TProperty> DisplayPropertyForListType<TInnerProperty>(Expression<Func<TInnerProperty, object>> propertyExpression) {
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
            
        if (attributes.Any(a => a is AdminDescriptionAttribute)) {
            var attribute = attributes.Single(a => a is AdminDescriptionAttribute) as AdminDescriptionAttribute;
            Description(attribute?.Description);
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