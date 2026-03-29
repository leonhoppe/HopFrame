using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using HopFrame.Core.Configuration;

namespace HopFrame.Core.Services.Implementation;

internal class EntityAccessor(IConfigAccessor accessor) : IEntityAccessor {
    
    public string? GetValue(object model, PropertyConfig property) {
        var value = GetValueRaw(model, property);
        if (value is null)
            return null;
        
        return FormatValue(value, property);
    }

    public object? GetValueRaw(object model, PropertyConfig property) {
        if (property.Getter is not null)
            return property.Getter.Invoke(model);

        if (property.Table.IsDictionary) {
            var dict = (IDictionary<string, object?>)model;
            if (dict.TryGetValue(property.Identifier, out var value))
                return value;

            return null;
        }
        
        var prop = model.GetType().GetProperty(property.Identifier);
        if (prop is null)
            return null;

        return prop.GetValue(model);
    }

    public string? FormatValue(object? value, PropertyConfig property, bool fromList = false) {
        if (value is null)
            return null;
        
        if ((property.PropertyType & PropertyType.List) != 0 && !fromList) {
            return (value as IList)!.Count.ToString();
        }

        if ((property.PropertyType & PropertyType.Relation) != 0) {
            var table = accessor.GetTableByIdentifier(property.RelationTable!);
            if (table?.PreferredProperty != null) {
                var tableProp = table.Properties.First(p => p.Identifier == table.PreferredProperty);
                return GetValue(value, tableProp);
            }
        }

        return value.ToString();
    }

    public void SetValue(object model, PropertyConfig property, object? value) {
        if (property.Setter is not null) {
            property.Setter.Invoke(model, value);
            return;
        }

        if (value?.GetType() != property.Type)
            value = Convert.ChangeType(value, property.Type);

        if (property.Table.IsDictionary) {
            var dict = (IDictionary<string, object?>)model;
            dict[property.Identifier] = value;
        }
        
        var prop = model.GetType().GetProperty(property.Identifier);
        if (prop is null)
            return;
        
        prop.SetValue(model, value);
    }

    public IEnumerable<string> ValidateProperty(PropertyConfig property, object? value) {
        var errors = new List<string>();

        if (value is not null && !value.GetType().IsAssignableTo(property.Type)) {
            errors.Add("Wrong type");
        }

        if ((property.PropertyType & PropertyType.Nullable) == 0 && value is null) {
            errors.Add(property.DisplayName + " cannot be empty");
        }

        if (value is string str) {
            if ((property.PropertyType & PropertyType.Nullable) == 0 && string.IsNullOrEmpty(str)) {
                errors.Add(property.DisplayName + " cannot be empty");
            }

            if ((PropertyType)((byte)property.PropertyType & 0x0F) == PropertyType.Email) {
                if (!Regex.IsMatch(str, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,10}$")) {
                    errors.Add("Address not valid");
                }
            }
        }

        if (property.Validator is not null && value is not null) {
            var validatorErrors = property.Validator.Invoke(value);
            errors.AddRange(validatorErrors.ToArray());
        }

        return errors;
    }
}