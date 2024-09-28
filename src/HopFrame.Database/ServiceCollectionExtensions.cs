using HopFrame.Database.Repositories;
using HopFrame.Database.Repositories.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Database;

public static class ServiceCollectionExtensions {

    public static IServiceCollection AddHopFrameRepositories<TDbContext>(this IServiceCollection services) where TDbContext : HopDbContextBase {
        services.AddScoped<IGroupRepository, GroupRepository<TDbContext>>();
        services.AddScoped<IPermissionRepository, PermissionRepository<TDbContext>>();
        services.AddScoped<IUserRepository, UserRepository<TDbContext>>();
        services.AddScoped<ITokenRepository, TokenRepository<TDbContext>>();

        return services;
    }

}