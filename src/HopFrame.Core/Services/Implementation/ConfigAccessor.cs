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
    
    public IHopFrameRepository LoadRepository(TableConfig table) {
        var repo = (IHopFrameRepository)services.GetRequiredService(table.RepositoryType);

        var genericRepoType = typeof(HopFrameRepository<>).MakeGenericType(table.TableType);
        if (table.RepositoryType.IsAssignableTo(genericRepoType)) {
            genericRepoType
                .GetMethod(nameof(HopFrameRepository<>.Initialize))!
                .Invoke(repo, [table]);
        }

        return repo;
    }
    
}