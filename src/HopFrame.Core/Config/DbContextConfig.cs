using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Config;

public interface ITableGroupConfig {
    public Type ContextType { get; }
    public List<TableConfig> Tables { get; }
    public HopFrameConfig ParentConfig { get; }
}

public class DbContextConfig : ITableGroupConfig {
    public Type ContextType { get; }
    public List<TableConfig> Tables { get; } = new();
    public HopFrameConfig ParentConfig { get; }

    public DbContextConfig(Type context, HopFrameConfig parentConfig) {
        ContextType = context;
        ParentConfig = parentConfig;

        foreach (var property in ContextType.GetProperties()) {
            if (!property.PropertyType.IsGenericType) continue;
            var innerType = property.PropertyType.GenericTypeArguments.First();
            var setType = typeof(DbSet<>).MakeGenericType(innerType);
            if (property.PropertyType != setType) continue;
            
            var table = new TableConfig(this, innerType, property.Name, Tables.Count);
            Tables.Add(table);
        }
    }
}

/// <summary>
/// A helper class for editing the <see cref="DbContextConfig"/>
/// </summary>
public sealed class DbContextConfigurator<TDbContext>(DbContextConfig config) {
    
    /// <summary>
    /// The Internal DbContext configuration that's modified by the helper functions
    /// </summary>
    public DbContextConfig InnerConfig { get; } = config;

    /// <summary>
    /// Configures the table of the DbContext using the provided configurator
    /// </summary>
    /// <param name="configurator">Used for configuring the table</param>
    /// <typeparam name="TModel">The model of the table for identifying the correct one</typeparam>
    /// <seealso cref="TableConfigurator{TModel}"/>
    public DbContextConfigurator<TDbContext> Table<TModel>(Action<TableConfigurator<TModel>> configurator) where TModel : class {
        var table = Table<TModel>();
        configurator.Invoke(table);
        return this;
    }
    
    /// <summary>
    /// Configures the table of the DbContext
    /// </summary>
    /// <typeparam name="TModel">The model of the table for identifying the correct one</typeparam>
    /// <returns>The configurator for the specified table</returns>
    /// <seealso cref="TableConfigurator{TModel}"/>
    public TableConfigurator<TModel> Table<TModel>() where TModel : class {
        var table = InnerConfig.Tables.Single(table => table.TableType == typeof(TModel));
        return new TableConfigurator<TModel>(table);
    }
    
}
