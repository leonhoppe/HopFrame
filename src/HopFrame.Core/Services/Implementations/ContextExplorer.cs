using HopFrame.Core.Config;

namespace HopFrame.Core.Services.Implementations;

internal sealed class ContextExplorer(HopFrameConfig config) : IContextExplorer {
    public IEnumerable<string> GetTableNames() {
        foreach (var context in config.Contexts) {
            foreach (var table in context.Tables) {
                if (table.Ignored) continue;
                yield return table.PropertyName;
            }
        }
    }
    
    public TableConfig? GetTable(string name) {
        foreach (var context in config.Contexts) {
            var table = context.Tables.FirstOrDefault(table => table.PropertyName == name);
            if (table is not null)
                return table;
        }

        return null;
    }
}