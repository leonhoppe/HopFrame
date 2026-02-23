using System.Linq.Expressions;
using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;
using HopFrame.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.EFCore;

/// Adds useful extensions to the <see cref="HopFrameConfigurator"/> to add managed <see cref="DbContext"/> repositories
public static class HopFrameConfiguratorExtensions {

    /// <summary>
    /// Adds managed repositories for the selected (or all if none provided) tables
    /// </summary>
    /// <param name="configurator">The configurator for the current <see cref="HopFrameConfig"/></param>
    /// <param name="includedTables">The tables that should be configured (if none are provided, all tables will be added)</param>
    /// <typeparam name="TDbContext">The already configured and injectable database context</typeparam>
    public static HopFrameConfigurator AddDbContext<TDbContext>(this HopFrameConfigurator configurator, params Expression<Func<TDbContext, object>>[] includedTables) where TDbContext : DbContext {
        var contextType = typeof(TDbContext);
        var properties = contextType.GetProperties()
            .Where(p => p.PropertyType.IsGenericType)
            .Where(p => p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>));

        if (includedTables.Length != 0) {
            properties = includedTables.Select(ExpressionHelper.GetPropertyInfo);
        }

        var tableIdentifiers = new List<string>();
        foreach (var tableProperty in properties) {
            var identifier = DbConfigPopulator.ConfigureRepository(configurator.Config, configurator.Services, contextType, tableProperty);
            tableIdentifiers.Add(identifier);
        }

        var createdTables = configurator.Config.Tables
            .Where(t => tableIdentifiers.Contains(t.Identifier))
            .ToArray();
        foreach (var createdTable in createdTables) {
            DbConfigPopulator.CheckForRelations(configurator.Config, createdTable);
        }
        
        return configurator;
    }
    
}