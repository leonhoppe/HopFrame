using System.Reflection;

namespace HopFrame.Core.Config;

public class RepositoryGroupConfig(Type repoType, PropertyInfo keyProperty, HopFrameConfig config) : ITableGroupConfig {
    public Type ContextType { get; } = repoType;

    public List<TableConfig> Tables { get; } = new();

    public HopFrameConfig ParentConfig { get; } = config;

    public PropertyInfo KeyProperty { get; } = keyProperty;
}