using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Services.Implementations;

internal sealed class ContextExplorer(HopFrameConfig config, IServiceProvider provider) : IContextExplorer {
    public IEnumerable<string> GetTableNames() {
        foreach (var context in config.Contexts) {
            foreach (var table in context.Tables) {
                if (table.Ignored) continue;
                yield return table.PropertyName;
            }
        }
    }
    
    public TableConfig? GetTable(string tableName) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.PropertyName.Equals(tableName, StringComparison.CurrentCultureIgnoreCase));
            if (table is not null)
                return table;
        }

        return null;
    }

    public TableConfig? GetTable(Type tableEntity) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.TableType == tableEntity);
            if (table is not null)
                return table;
        }

        return null;
    }

    public ITableManager? GetTableManager(string tableName) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.PropertyName == tableName);
            if (table is null) continue;
            
            var dbContext = provider.GetService(context.ContextType) as DbContext;
            if (dbContext is null) return null;

            var type = typeof(TableManager<>).MakeGenericType(table.TableType);
            return Activator.CreateInstance(type, dbContext, table, this) as ITableManager;
        }

        return null;
    }

}