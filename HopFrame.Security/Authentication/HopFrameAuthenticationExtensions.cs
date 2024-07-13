using HopFrame.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace HopFrame.Security.Authentication;

public static class HopFrameAuthenticationExtensions {

    public static AuthenticationBuilder AddHopFrameAuthentication<TDbContext>(this IServiceCollection service) where TDbContext : HopDbContextBase {
        return service.AddAuthentication(HopFrameAuthentication<TDbContext>.SchemeName).AddScheme<AuthenticationSchemeOptions, HopFrameAuthentication<TDbContext>>(HopFrameAuthentication<TDbContext>.SchemeName, _ => {});
    }
    
}