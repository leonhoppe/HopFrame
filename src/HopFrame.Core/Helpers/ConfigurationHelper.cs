using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.CompilerServices;
using HopFrame.Core.Configuration;
// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract

namespace HopFrame.Core.Helpers;

internal static class ConfigurationHelper {

    public static TableConfig InitializeTable(HopFrameConfig global, Type repositoryType, Type modelType) {
        var identifier = modelType.Name;

        if (global.Tables.Any(t => t.Identifier == identifier))
            identifier = Guid.NewGuid().ToString();
        
        var config = new TableConfig {
            RepositoryType = repositoryType,
            TableType = modelType,
            Identifier = identifier,
            Route = modelType.Name.ToLower() + 's',
            DisplayName = modelType.Name + 's',
            OrderIndex = global.Tables.Count
        };
        
        foreach (var property in modelType.GetProperties()) {
            config.Properties.Add(InitializeProperty(config, property));
        }

        return config;
    }

    public static PropertyConfig InitializeProperty(TableConfig table, PropertyInfo property) {
        var identifier = property.Name;

        if (table.Properties.Any(p => p.Identifier == identifier))
            identifier = Guid.NewGuid().ToString();

        var config = new PropertyConfig {
            Identifier = identifier,
            Type = property.PropertyType,
            DisplayName = property.Name,
            OrderIndex = table.Properties.Count,
            PropertyType = InferPropertyType(property.PropertyType, property),
            Table = table
        };

        if (property.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)))
            table.PreferredProperty = config.Identifier;

        return config;
    }

    public static PropertyType InferPropertyType(Type realType, PropertyInfo info) {
        byte modifiers = 0;

        if (Nullable.GetUnderlyingType(realType) != null) {
            modifiers |= (byte)PropertyType.Nullable;
            realType = Nullable.GetUnderlyingType(realType)!;
        }

        if (info.CustomAttributes.Any(a => a.AttributeType == typeof(NullableAttribute))) {
            modifiers |= (byte)PropertyType.Nullable;
        }

        if ((realType.IsAssignableTo(typeof(IEnumerable)) || realType.IsAssignableTo(typeof(IEnumerable<>))) && realType != typeof(string)) {
            modifiers |= (byte)PropertyType.List;
        }

        if (realType.IsGenericType) {
            realType = realType.GenericTypeArguments.First();
        }
        
        var type = PropertyType.Text;
        var realTypeCode = Type.GetTypeCode(realType);

        if (realTypeCode == TypeCode.Boolean)
            type = PropertyType.Boolean;

        if (realTypeCode == TypeCode.DateTime)
            type = PropertyType.DateTime;

        if (realType == typeof(DateOnly))
            type = PropertyType.DateOnly;

        if (realType == typeof(TimeOnly))
            type = PropertyType.TimeOnly;

        if (realType.IsEnum)
            type = PropertyType.Enum;

        if (realType.IsNumeric())
            type = PropertyType.Numeric;

        if (info.CustomAttributes.Any(a => a.AttributeType == typeof(EmailAddressAttribute)))
            type = PropertyType.Email;

        return (PropertyType)((byte)type | modifiers);
    }

    public static IEnumerable<string> ValidateTable(HopFrameConfig global, TableConfig config) {
        if (global.Tables.Any(t => t.Identifier == config.Identifier))
            yield return $"Table identifier '{config.Identifier}' is not unique";

        if (config.TableType is null)
            yield return "TableType cannot be null";

        if (config.RepositoryType is null)
            yield return "RepositoryType cannot be null";

        if (config.Route is null)
            yield return "Route cannot be null";

        if (config.DisplayName is null)
            yield return "DisplayName cannot be null";
        
        foreach (var property in config.Properties) {
            if (config.Properties.Count(p => p.Identifier == property.Identifier) > 1)
                yield return $"Property identifier '{property.Identifier}' is not unique";

            if (property.DisplayName is null)
                yield return $"Property '{property.Identifier}': DisplayName cannot be null";
            
            if (property.Type is null)
                yield return $"Property '{property.Identifier}': Type cannot be null";
        }
    }
    
}