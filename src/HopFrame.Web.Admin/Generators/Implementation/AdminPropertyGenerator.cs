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
        return this;
    }

    public IAdminPropertyGenerator Bold(bool isBold = true) {
        _property.Bold = isBold;
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

    public AdminPageProperty Compile() {
        _property.DisplayName ??= _property.Name;

        return _property;
    }
}