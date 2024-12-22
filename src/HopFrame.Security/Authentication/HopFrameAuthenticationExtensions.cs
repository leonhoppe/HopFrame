using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authentication.OpenID.Implementation;
using HopFrame.Security.Authentication.OpenID.Options;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using HopFrame.Security.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Security.Authentication;

public static class HopFrameAuthenticationExtensions {
    /// <summary>
    /// Configures the WebApplication to use the authentication and authorization of the HopFrame API
    /// </summary>
    /// <param name="service">The service provider to add the services to</param>
    /// <param name="configuration">The configuration used to configure HopFrame authentication</param>
    /// <returns></returns>
    public static IServiceCollection AddHopFrameAuthentication(this IServiceCollection service, ConfigurationManager configuration) {
        service.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddScoped<ITokenContext, TokenContextImplementor>();
        
        service.AddHttpClient();
        service.AddMemoryCache();
        service.AddScoped<IOpenIdAccessor, OpenIdAccessor>();
        
        service.AddOptionsFromConfiguration<HopFrameAuthenticationOptions>(configuration);
        service.AddOptionsFromConfiguration<AdminPermissionOptions>(configuration);
        service.AddOptionsFromConfiguration<OpenIdOptions>(configuration);
        
        service.AddAuthentication(HopFrameAuthentication.SchemeName).AddScheme<AuthenticationSchemeOptions, HopFrameAuthentication>(HopFrameAuthentication.SchemeName, _ => {});
        service.AddAuthorization();

        return service;
    }
    
}