using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HopFrame.Core.Services.Implementations;

internal sealed class ContextExplorer(HopFrameConfig config, IServiceProvider provider, ILogger<ContextExplorer> logger) : IContextExplorer {
    public IEnumerable<string> GetTableNames() {
        foreach (var context in config.Contexts) {
            foreach (var table in context.Tables) {
                if (table.Ignored) continue;
                yield return table.DisplayName;
            }
        }
    }
    
    public TableConfig? GetTable(string tableDisplayName) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.DisplayName.Equals(tableDisplayName, StringComparison.CurrentCultureIgnoreCase));
            if (table is null) continue;
            
            SeedTableData(table);
            return table;
        }

        return null;
    }

    public TableConfig? GetTable(Type tableEntity) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.TableType == tableEntity);
            if (table is null) continue;
            
            SeedTableData(table);
            return table;
        }

        return null;
    }

    public ITableManager? GetTableManager(string tablePropertyName) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.PropertyName == tablePropertyName);
            if (table is null) continue;
            
            var dbContext = provider.GetService(context.ContextType) as DbContext;
            if (dbContext is null) return null;

            var type = typeof(TableManager<>).MakeGenericType(table.TableType);
            return Activator.CreateInstance(type, dbContext, table, this) as ITableManager;
        }

        return null;
    }

    private void SeedTableData(TableConfig table) {
        if (table.Seeded) return;
        var dbContext = (provider.GetService(table.ContextConfig.ContextType) as DbContext)!;
        var entity = dbContext.Model.FindEntityType(table.TableType)!;
        
        foreach (var key in entity.GetForeignKeys()) {
            var propConfig = table.Properties
                .SingleOrDefault(prop => prop.Info == key.DependentToPrincipal?.PropertyInfo);
            if (propConfig is null) continue;
            propConfig.IsRelation = true;
        }
        
        foreach (var property in entity.GetProperties()) {
            var propConfig = table.Properties
                .SingleOrDefault(prop => prop.Info == property.PropertyInfo);
            if (propConfig is null) continue;
            propConfig.IsRequired = !property.IsNullable;
        }
        
        logger.LogInformation("Extracted information for table '" + table.PropertyName + "'");
        table.Seeded = true;
    }

}