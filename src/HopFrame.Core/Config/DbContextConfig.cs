using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Config;

public class DbContextConfig {
    public Type ContextType { get; }
    public List<TableConfig> Tables { get; init; } = new();

    public DbContextConfig(Type context) {
        ContextType = context;

        foreach (var property in ContextType.GetProperties()) {
            if (!property.PropertyType.IsGenericType) continue;
            var innerType = property.PropertyType.GenericTypeArguments.First();
            var setType = typeof(DbSet<>).MakeGenericType(innerType);
            if (property.PropertyType != setType) continue;
            
            var table = new TableConfig(this, innerType, property.Name);
            Tables.Add(table);
        }
    }
}

public class DbContextConfig<TDbContext>(DbContextConfig config) {
    public DbContextConfig InnerConfig { get; } = config;

    public DbContextConfig<TDbContext> Table<TModel>(Action<TableConfig<TModel>> configurator) where TModel : class {
        var table = Table<TModel>();
        configurator.Invoke(table);
        return this;
    }
    
    public TableConfig<TModel> Table<TModel>() where TModel : class {
        var table = InnerConfig.Tables.Single(table => table.TableType == typeof(TModel));
        return new TableConfig<TModel>(table);
    }
    
}
