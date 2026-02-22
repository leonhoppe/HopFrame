using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Core.Services.Implementation;

internal sealed class ConfigAccessor(HopFrameConfig config, IServiceProvider services) : IConfigAccessor {
    
    public TableConfig? GetTableByIdentifier(string identifier) {
        return config.Tables.FirstOrDefault(t => t.Identifier == identifier);
    }
    
    public TableConfig? GetTableByRoute(string route) {
        return config.Tables.FirstOrDefault(t => t.Route == route);
    }
    
    public TableConfig? GetTableByType(Type type) {
        return config.Tables.FirstOrDefault(t => t.TableType == type);
    }
    
    public IHopFrameRepository LoadRepository(TableConfig table) {
        return (IHopFrameRepository)services.GetRequiredService(table.RepositoryType);
    }
    
}