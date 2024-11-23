using HopFrame.Security.Claims;
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
    public static IServiceCollection AddHopFrameAuthentication(this IServiceCollection service) {
        service.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddScoped<ITokenContext, TokenContextImplementor>();
        
        service.AddAuthentication(HopFrameAuthentication.SchemeName).AddScheme<AuthenticationSchemeOptions, HopFrameAuthentication>(HopFrameAuthentication.SchemeName, _ => {});
        service.AddAuthorization();

        return service;
    }
    
}