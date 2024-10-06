using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Members;
using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Generators.Implementation;

internal sealed class AdminPropertyGenerator(string name, Type type) : IAdminPropertyGenerator, IGenerator<AdminPageProperty> {
    
    private readonly AdminPageProperty _property = new() {
        Name = name,
        Type = type
    };

    public IAdminPropertyGenerator Sortable(bool sortable) {
        _property.Sortable = sortable;
        return this;
    }

    public IAdminPropertyGenerator Editable(bool editable) {
        _property.Editable = editable;
        return this;
    }

    public IAdminPropertyGenerator DisplayValueWhileEditing(bool display) {
        _property.EditDisplayValue = display;
        return this;
    }

    public IAdminPropertyGenerator DisplayInListing(bool display = true) {
        _property.DisplayInListing = display;
        _property.Sortable = false;
        return this;
    }

    public IAdminPropertyGenerator Ignore(bool ignore = false) {
        _property.Ignore = ignore;
        return this;
    }

    public IAdminPropertyGenerator Generated(bool generated = true) {
        _property.Generated = generated;
        return this;
    }

    public IAdminPropertyGenerator DisplayName(string displayName) {
        _property.DisplayName = displayName;
        return this;
    }

    public IAdminPropertyGenerator Description(string description) {
        _property.Description = description;
        return this;
    }

    public IAdminPropertyGenerator Prefix(string prefix) {
        _property.Prefix = prefix;
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
        }

        if (attributes.Any(a => a is AdminUnsortableAttribute))
            Sortable(false);

        if (attributes.Any(a => a is AdminUneditableAttribute))
            Editable(false);
        
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

        if (attributes.Any(a => a is AdminPrefixAttribute)) {
            var attribute = attributes.Single(a => a is AdminPrefixAttribute) as AdminPrefixAttribute;
            Prefix(attribute?.Prefix);
        }
    }
}