using System.Reflection;
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
            Route = modelType.Name.ToLower(),
            DisplayName = modelType.Name,
            OrderIndex = global.Tables.Count
        };
        
        foreach (var property in modelType.GetProperties()) {
            config.Properties.Add(InitializeProperty(config, property));
        }

        return config;
    }

    private static PropertyConfig InitializeProperty(TableConfig table, PropertyInfo property) {
        var identifier = property.Name;

        if (table.Properties.Any(p => p.Identifier == identifier))
            identifier = Guid.NewGuid().ToString();

        var config = new PropertyConfig {
            Identifier = identifier,
            Type = property.PropertyType,
            DisplayName = property.Name,
            OrderIndex = table.Properties.Count
        };

        return config;
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