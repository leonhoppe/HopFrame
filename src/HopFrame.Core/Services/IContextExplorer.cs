using HopFrame.Core.Config;

namespace HopFrame.Core.Services;

public interface IContextExplorer {
    public IEnumerable<string> GetTableNames();
    public TableConfig? GetTable(string name);
}