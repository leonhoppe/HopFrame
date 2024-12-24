using HopFrame.Database;
using HopFrame.Database.Models;
using HopFrame.Security.Authentication.OpenID;
using HopFrame.Security.Authentication.OpenID.Implementation;
using HopFrame.Security.Authentication.OpenID.Options;
using HopFrame.Security.Authorization;
using HopFrame.Security.Claims;
using HopFrame.Security.Models;
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
    /// <param name="services">The service provider to add the services to</param>
    /// <param name="configuration">The configuration used to configure HopFrame authentication</param>
    /// <param name="config">Configuration for how the HopFrame services are set up</param>
    /// <returns></returns>
    public static IServiceCollection AddHopFrameAuthentication(this IServiceCollection services, ConfigurationManager configuration, HopFrameConfig config = null) {
        config ??= new HopFrameConfig();

        services.AddSingleton(config);
        services.AddScoped(typeof(ICacheProvider), config.CacheProvider);
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<ITokenContext, TokenContextImplementor>();

        if (config.CacheProvider == typeof(MemoryCacheProvider))
            services.AddMemoryCache();
        
        services.AddHttpClient<OpenIdAccessor>();
        services.AddScoped<IOpenIdAccessor, OpenIdAccessor>();
        
        services.AddOptionsFromConfiguration<HopFrameAuthenticationOptions>(configuration);
        services.AddOptionsFromConfiguration<AdminPermissionOptions>(configuration);
        services.AddOptionsFromConfiguration<OpenIdOptions>(configuration);
        
        services.AddAuthentication(HopFrameAuthentication.SchemeName).AddScheme<AuthenticationSchemeOptions, HopFrameAuthentication>(HopFrameAuthentication.SchemeName, _ => {});
        services.AddAuthorization();
        
        HopDbContextBase.SaveHandlers.Add(context => {
            var section = configuration.GetSection("HopFrame:Authentication");
            var accessToken = section?.GetSection("AccessToken")?.Get<HopFrameAuthenticationOptions.TokenTime>()?.ConstructTimeSpan ?? new HopFrameAuthenticationOptions().AccessTokenTime;
            var refreshToken = section?.GetSection("RefreshToken")?.Get<HopFrameAuthenticationOptions.TokenTime>()?.ConstructTimeSpan ?? new HopFrameAuthenticationOptions().RefreshTokenTime;

            var now = DateTime.Now;
            var accessTokenExpiry = now - accessToken;
            var refreshTokenExpiry = now - refreshToken;
            var invalidTokens = context.Tokens
                .Where(t => 
                    (t.Type == Token.AccessTokenType && t.CreatedAt < accessTokenExpiry) ||
                    (t.Type == Token.RefreshTokenType && t.CreatedAt < refreshTokenExpiry))
                .ToList();
            context.Tokens.RemoveRange(invalidTokens);
        });

        return services;
    }
    
}