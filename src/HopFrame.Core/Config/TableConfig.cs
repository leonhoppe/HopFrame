namespace HopFrame.Core.Config;

public class TableConfig(DbContextConfig config, Type tableType, string propertyName) {
    public Type TableType { get; } = tableType;
    public string PropertyName { get; } = propertyName;
    public DbContextConfig ContextConfig { get; } = config;
    public bool Ignored { get; set; }
}

public class TableConfig<TModel>(TableConfig innerConfig) {

    public TableConfig<TModel> Ignore() {
        innerConfig.Ignored = true;
        return this;
    }
    
}
