using System.Text.Json;
using HopFrame.Security.Authentication.OpenID.Models;
using HopFrame.Security.Authentication.OpenID.Options;
using Microsoft.Extensions.Options;

namespace HopFrame.Security.Authentication.OpenID.Implementation;

internal class OpenIdAccessor(IHttpClientFactory clientFactory, IOptions<OpenIdOptions> options) : IOpenIdAccessor {
    public async Task<OpenIdConfiguration> LoadConfiguration() {
        var client = clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, Path.Combine(options.Value.Issuer, ".well-known/openid-configuration"));
        var response = await client.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
            return null;
        
        return await JsonSerializer.DeserializeAsync<OpenIdConfiguration>(await response.Content.ReadAsStreamAsync());
    }

    public async Task<OpenIdToken> RequestToken(string code) {
        var configuration = await LoadConfiguration();

        var client = clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, configuration.TokenEndpoint) {
            Content = new FormUrlEncodedContent(new Dictionary<string, string> {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", options.Value.Callback },
                { "client_id", options.Value.ClientId },
                { "client_secret", options.Value.ClientSecret }
            })
        };
        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        return await JsonSerializer.DeserializeAsync<OpenIdToken>(await response.Content.ReadAsStreamAsync());
    }

    public async Task<string> ConstructAuthUri(string state = null) {
        var configuration = await LoadConfiguration();
        return $"{configuration.AuthorizationEndpoint}?response_type=code&client_id={options.Value.ClientId}&redirect_uri={options.Value.Callback}&scope=openid%20profile%20email&state={state}";
    }

    public async Task<OpenIdIntrospection> InspectToken(string token) {
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

        return await JsonSerializer.DeserializeAsync<OpenIdIntrospection>(await response.Content.ReadAsStreamAsync());
    }
}