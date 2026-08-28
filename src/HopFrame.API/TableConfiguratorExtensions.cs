using System.Linq.Expressions;
using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;
using HopFrame.Core.EFCore;
using HopFrame.Core.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.API;

public static class TableConfiguratorExtensions {

    internal static readonly Dictionary<string, IEnumerable<string>> TableIdentifiers = new();

    public static TableConfigurator<TModel> HasIdentifier<TModel, TProp>(this TableConfigurator<TModel> configurator, params Expression<Func<TModel, TProp>>[] propertyExpression) where TModel : class {
        List<string> identifiers = new();
        
        foreach (var expression in propertyExpression) {
            var propertyName = ExpressionHelper.GetPropertyInfo(expression).Name;
            var prop = configurator.Config.Properties.First(p => p.Identifier == propertyName);
            identifiers.Add(prop.Identifier);
        }
        
        TableIdentifiers.Add(configurator.Config.Identifier, identifiers.ToArray());
        return configurator;
    }
    
    public static TableConfigurator<TModel> HasIdentifier<TModel>(this TableConfigurator<TModel> configurator, params string[] properties) where TModel : class {
        if (properties.Any(i => configurator.Config.Properties.All(p => p.Identifier != i)))
            throw new ArgumentException($"Property identifiers do not match table properties '{configurator.Config.DisplayName}'");
        
        TableIdentifiers.Add(configurator.Config.Identifier, properties);
        return configurator;
    }

    internal static void ExtractIdentifiers(this WebApplication app, HopFrameConfig config) {
        var tables = config.Tables.Where(t => !TableIdentifiers.ContainsKey(t.Identifier)).ToArray();
        var efRepoType = typeof(EfCoreRepository<,>);
        using var scope = app.Services.CreateAsyncScope();

        foreach (var table in tables) {
            if (!table.RepositoryType.IsGenericType || table.RepositoryType.GetGenericTypeDefinition() != efRepoType)
                continue;

            var contextType = table.RepositoryType.GenericTypeArguments[1];
            var context = (scope.ServiceProvider.GetRequiredService(contextType) as DbContext)!;
            var key = context.Model.FindEntityType(table.TableType)!.FindPrimaryKey();
            if (key is null)
                continue;

            var keyProperties = key.Properties
                .Select(k => table.Properties.First(p => p.Identifier == k.PropertyInfo!.Name))
                .Select(p => p.Identifier)
                .ToArray();
            
            TableIdentifiers.Add(table.Identifier, keyProperties);
        }
    }
    
}