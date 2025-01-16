using HopFrame.Core.Config;

namespace HopFrame.Core.Services;

public interface IContextExplorer {
    public IEnumerable<TableConfig> GetTables();
    public TableConfig? GetTable(string tableDisplayName);
    public TableConfig? GetTable(Type tableEntity);
    public ITableManager? GetTableManager(string tablePropertyName);
}