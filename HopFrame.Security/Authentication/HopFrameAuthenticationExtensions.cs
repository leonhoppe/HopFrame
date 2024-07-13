using HopFrame.Database;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HopFrame.Security.Authentication;

public static class HopFrameAuthenticationExtensions {

    public static AuthenticationBuilder AddHopFrameAuthentication<TDbContext>(this IServiceCollection service) where TDbContext : HopDbContextBase {
        service.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddScoped<ITokenContext, TokenContextImplementor<TDbContext>>();
        return service.AddAuthentication(HopFrameAuthentication<TDbContext>.SchemeName).AddScheme<AuthenticationSchemeOptions, HopFrameAuthentication<TDbContext>>(HopFrameAuthentication<TDbContext>.SchemeName, _ => {});
    }
    
}