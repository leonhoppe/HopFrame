using HopFrame.Database;
using HopFrame.Security.Claims;
using HopFrame.Security.Services;
using HopFrame.Security.Services.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Security.Authentication;

public static class HopFrameAuthenticationExtensions {

    /// <summary>
    /// Configures the WebApplication to use the authentication and authorization of the HopFrame API
    /// </summary>
    /// <param name="service">The service provider to add the services to</param>
    /// <typeparam name="TDbContext">The database object that saves all entities that are important for the security api</typeparam>
    /// <returns></returns>
    public static AuthenticationBuilder AddHopFrameAuthentication<TDbContext>(this IServiceCollection service) where TDbContext : HopDbContextBase {
        service.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddScoped<ITokenContext, TokenContextImplementor<TDbContext>>();
        service.AddScoped<IPermissionService, PermissionService<TDbContext>>();
        service.AddScoped<IUserService, UserService<TDbContext>>();
        
        return service.AddAuthentication(HopFrameAuthentication<TDbContext>.SchemeName).AddScheme<AuthenticationSchemeOptions, HopFrameAuthentication<TDbContext>>(HopFrameAuthentication<TDbContext>.SchemeName, _ => {});
    }
    
}