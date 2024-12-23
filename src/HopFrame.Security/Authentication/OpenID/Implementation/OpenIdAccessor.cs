using System.Text.Json;
using HopFrame.Security.Authentication.OpenID.Models;
using HopFrame.Security.Authentication.OpenID.Options;
using HopFrame.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace HopFrame.Security.Authentication.OpenID.Implementation;

internal class OpenIdAccessor(IHttpClientFactory clientFactory, IOptions<OpenIdOptions> options, IHttpContextAccessor accessor, IMemoryCache cache) : IOpenIdAccessor {
    private const string ConfigurationCacheKey = "HopFrame:OpenID:Configuration";
    private const string AuthCodeCacheKey = "HopFrame:OpenID:Code:";
    private const string TokenCacheKey = "HopFrame:OpenID:Token:";
    
    public async Task<OpenIdConfiguration> LoadConfiguration() {
        if (options.Value.Cache.Enabled && options.Value.Cache.Configuration.Enabled && cache.TryGetValue(ConfigurationCacheKey, out object cachedConfiguration)) {
            return cachedConfiguration as OpenIdConfiguration;
        }
        
        var client = clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, Path.Combine(options.Value.Issuer, ".well-known/openid-configuration").Replace("\\", "/"));
        var response = await client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
            return null;
        
        var config = await JsonSerializer.DeserializeAsync<OpenIdConfiguration>(await response.Content.ReadAsStreamAsync());
        
        if (options.Value.Cache.Enabled && options.Value.Cache.Configuration.Enabled)
            cache.Set(ConfigurationCacheKey, config, options.Value.Cache.Configuration.TTL.ConstructTimeSpan);
        
        return config;
    }

    public async Task<OpenIdToken> RequestToken(string code) {
        if (options.Value.Cache.Enabled && options.Value.Cache.Auth.Enabled && cache.TryGetValue(AuthCodeCacheKey + code, out object cachedToken)) {
            return cachedToken as OpenIdToken;
        }
        
        var protocol = accessor.HttpContext!.Request.IsHttps ? "https" : "http";
        var callback = options.Value.Callback ?? Path.Combine($"{protocol}://{accessor.HttpContext!.Request.Host.Value}", IOpenIdAccessor.DefaultCallback).Replace("\\", "/");
        
        var configuration = await LoadConfiguration();

        var client = clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, configuration.TokenEndpoint) {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", callback },
                { "client_id", options.Value.ClientId },
                { "client_secret", options.Value.ClientSecret }
            })
        };
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var token = await JsonSerializer.DeserializeAsync<OpenIdToken>(await response.Content.ReadAsStreamAsync());
        
        if (options.Value.Cache.Enabled && options.Value.Cache.Auth.Enabled)
            cache.Set(AuthCodeCacheKey + code, token, options.Value.Cache.Auth.TTL.ConstructTimeSpan);
        
        return token;
    }

    public async Task<string> ConstructAuthUri(string state = null) {
        var protocol = accessor.HttpContext!.Request.IsHttps ? "https" : "http";
        var callback = options.Value.Callback ?? Path.Combine($"{protocol}://{accessor.HttpContext!.Request.Host.Value}", IOpenIdAccessor.DefaultCallback).Replace("\\", "/");
        
        var configuration = await LoadConfiguration();
        return $"{configuration.AuthorizationEndpoint}?response_type=code&client_id={options.Value.ClientId}&redirect_uri={callback}&scope=openid%20profile%20email%20offline_access&state={state}";
    }

    public async Task<OpenIdIntrospection> InspectToken(string token) {
        if (options.Value.Cache.Enabled && options.Value.Cache.Inspection.Enabled && cache.TryGetValue(TokenCacheKey + token, out object cachedToken)) {
            return cachedToken as OpenIdIntrospection;
        }
        
        var configuration = await LoadConfiguration();

        var client = clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, configuration.IntrospectionEndpoint) {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "token", token },
                { "client_id", options.Value.ClientId },
                { "client_secret", options.Value.ClientSecret }
            })
        };
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var introspection = await JsonSerializer.DeserializeAsync<OpenIdIntrospection>(await response.Content.ReadAsStreamAsync());
        
        if (options.Value.Cache.Enabled && options.Value.Cache.Inspection.Enabled)
            cache.Set(TokenCacheKey + token, introspection, options.Value.Cache.Inspection.TTL.ConstructTimeSpan);
        
        return introspection;
    }

    public async Task<OpenIdToken> RefreshAccessToken(string refreshToken) {
        var configuration = await LoadConfiguration();
        
        var client = clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, configuration.TokenEndpoint) {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", options.Value.ClientId },
                { "client_secret", options.Value.ClientSecret }
            })
        };
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await JsonSerializer.DeserializeAsync<OpenIdToken>(await response.Content.ReadAsStreamAsync());
    }

    public void SetAuthenticationCookies(OpenIdToken token) {
        if (token.AccessToken is not null)
            accessor.HttpContext!.Response.Cookies.Append(ITokenContext.AccessTokenType, token.AccessToken, new CookieOptions {
                MaxAge = TimeSpan.FromSeconds(token.ExpiresIn),
                HttpOnly = false,
                Secure = true
            });
        
        if (token.RefreshToken is not null)
            accessor.HttpContext!.Response.Cookies.Append(ITokenContext.RefreshTokenType, token.RefreshToken, new CookieOptions {
                MaxAge = options.Value.RefreshToken.ConstructTimeSpan,
                HttpOnly = false,
                Secure = true
            });
    }
}