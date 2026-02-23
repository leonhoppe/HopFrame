using System.Reflection;
using HopFrame.Core.Configuration;
using HopFrame.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core.EFCore;

internal static class DbConfigPopulator {

    public static string ConfigureRepository(HopFrameConfig global, IServiceCollection services, Type contextType, PropertyInfo tableProperty) {
        var modelType = tableProperty.PropertyType.GenericTypeArguments.First();
        var repoType = typeof(EfCoreRepository<,>).MakeGenericType(modelType, contextType);

        services.AddScoped(repoType);

        var table = ConfigurationHelper.InitializeTable(global, repoType, modelType);
        global.Tables.Add(table);
        return table.Identifier;
    }

    public static void CheckForRelations(HopFrameConfig global, TableConfig table) {
        foreach (var property in table.Properties) {
            var type = property.Type;

            if ((property.PropertyType & PropertyType.List) != 0 && type.IsGenericType) {
                type = type.GenericTypeArguments.First();
            }

            if (global.Tables.Any(t => t.TableType == type))
                property.PropertyType |= PropertyType.Relation;
        }
    }
    
}