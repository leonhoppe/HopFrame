using System.Text.Json;
using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HopFrame.Core.Services.Implementations;

internal sealed class ContextExplorer(HopFrameConfig config, IServiceProvider provider, ILogger<ContextExplorer> logger) : IContextExplorer {
    public IEnumerable<TableConfig> GetTables() {
        foreach (var context in config.Contexts) {
            foreach (var table in context.Tables) {
                if (table.Ignored) continue;
                yield return table;
            }
        }
    }
    
    public TableConfig? GetTable(string tableDisplayName) {
        foreach (var context in config.Contexts) {
            var table = context.Tables
                .Where(t => !t.Ignored)
                .FirstOrDefault(table => table.DisplayName.Equals(tableDisplayName, StringComparison.CurrentCultureIgnoreCase));
            if (table is null) continue;
            
            SeedTableData(table);
            return table;
        }

        return null;
    }

    public TableConfig? GetTable(Type tableEntity) {
        foreach (var context in config.Contexts) {
            var table = context.Tables
                .Where(t => !t.Ignored)
                .FirstOrDefault(table => table.TableType == tableEntity);
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
            
            var repo = provider.GetService(context.ContextType);
            if (repo is null) return null;

            if (context is DbContextConfig) {
                var type = typeof(TableManager<>).MakeGenericType(table.TableType);
                return Activator.CreateInstance(type, (DbContext)repo, table, this, provider, provider.GetRequiredService<ISearchExpressionBuilder>()) as ITableManager;
            }
            
            if (context is RepositoryGroupConfig repoConfig) {
                var type = typeof(RepositoryTableManager<,>).MakeGenericType(table.TableType, repoConfig.KeyProperty.PropertyType);
                return Activator.CreateInstance(type, repo, this, provider, provider.GetRequiredService<ISearchExpressionBuilder>()) as ITableManager;
            }
        }

        return null;
    }

    public ITableManager? GetTableManager(Type tableType) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.TableType == tableType);
            if (table is null) continue;
            
            var repo = provider.GetService(context.ContextType);
            if (repo is null) return null;

            if (context is DbContextConfig) {
                var type = typeof(TableManager<>).MakeGenericType(table.TableType);
                return Activator.CreateInstance(type, (DbContext)repo, table, this, provider, provider.GetRequiredService<ISearchExpressionBuilder>()) as ITableManager;
            }
            
            if (context is RepositoryGroupConfig repoConfig) {
                var type = typeof(RepositoryTableManager<,>).MakeGenericType(table.TableType, repoConfig.KeyProperty.PropertyType);
                return Activator.CreateInstance(type, repo, this, provider, provider.GetRequiredService<ISearchExpressionBuilder>()) as ITableManager;
            }
        }

        return null;
    }

    private void SeedTableData(TableConfig table) {
        if (table.Seeded) return;
        if (table.ContextConfig is not DbContextConfig) return;
        var dbContext = (provider.GetRequiredService(table.ContextConfig.ContextType) as DbContext)!;
        var entity = dbContext.Model.FindEntityType(table.TableType)!;
        
        foreach (var propertyConfig in table.Properties) {
            if (propertyConfig.IsVirtualProperty) continue;
            if (propertyConfig.IsRelation) continue;
            
            var prop = entity.FindProperty(propertyConfig.Info.Name);
            if (prop is not null) continue;
            
            var nav = entity.FindNavigation(propertyConfig.Info.Name);
            if (nav is null) {
                if (!propertyConfig.Info.PropertyType.IsGenericType) continue;
                var relationType = propertyConfig.Info.PropertyType.GenericTypeArguments[0];

                var isRelation = entity.Model.GetEntityTypes()
                    .Select(e => e.GetForeignKeys())
                    .Any(keys => keys
                        .Select(k => k.PrincipalEntityType.ClrType)
                        .Any(t => t == relationType || t == table.TableType));
                if (!isRelation) continue;
                
                propertyConfig.IsRelation = true;
                propertyConfig.IsRequired = false;
                propertyConfig.IsEnumerable = true;
                
                continue;
            }
            
            propertyConfig.IsRelation = true;
            propertyConfig.IsRequired = nav.ForeignKey.IsRequired;
            propertyConfig.IsEnumerable = nav.IsCollection;
        }
        
        foreach (var property in entity.GetProperties()) {
            var propConfig = table.Properties
                .Where(prop => !prop.IsVirtualProperty)
                .SingleOrDefault(prop => prop.Info == property.PropertyInfo);
            if (propConfig is null || propConfig.IsRequired) continue;
            propConfig.IsRequired = !property.IsNullable;
        }
        
        logger.LogInformation("Extracted information for table '" + table.PropertyName + "'");
        table.Seeded = true;
    }

}