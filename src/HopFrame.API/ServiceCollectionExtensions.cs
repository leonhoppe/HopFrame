using System.Diagnostics.CodeAnalysis;
using HopFrame.API.Endpoints;
using HopFrame.Core;
using HopFrame.Core.Configuration;
using HopFrame.Core.Configurators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace HopFrame.API;

public static class ServiceCollectionExtensions {

    private static string BaseUrl { get; set; } = null!;
    private static HopFrameConfig Config { get; set; } = null!;

    /// <inheritdoc cref="HopFrame.Core.ServiceCollectionExtensions.AddHopFrameServices"/>
    public static IServiceCollection AddHopFrame(this IServiceCollection services, Action<HopFrameConfigurator> configurator) {
        services.AddHopFrameServices(configurator);
        
        services.ConfigureAll<OpenApiOptions>(options => {
            options.AddDocumentTransformer((document, context, ct) => {
                var config = context.ApplicationServices?.GetService<HopFrameConfig>();
                if (config == null || document.Components?.Schemas == null)
                    return Task.CompletedTask;

                foreach (var table in config.Tables) {
                    if (document.Components.Schemas.TryGetValue(table.TableType.Name, out var schema) && schema.Properties != null) {
                        var originalProperties = new Dictionary<string, IOpenApiSchema>();

                        foreach (var prop in table.TableType.GetProperties()) {
                            var existing = schema.Properties.FirstOrDefault(p => 
                                string.Equals(p.Key, prop.Name, StringComparison.OrdinalIgnoreCase));

                            if (existing.Value is not null) {
                                originalProperties[prop.Name] = existing.Value;
                            }
                        }

                        if (originalProperties.Count > 0) {
                            schema.Properties.Clear();
                            foreach (var (key, value) in originalProperties) {
                                schema.Properties.Add(key, value);
                            }
                        }
                    }
                }

                return Task.CompletedTask;
            });
        });

        return services;
    }

    /// <summary>
    /// Maps all HopFrame REST endpoints using the provided baseUrl
    /// </summary>
    /// <param name="app">The application that hosts the endpoints</param>
    /// <param name="baseUrl">The base url, every HopFrame url should start with</param>
    public static WebApplication MapHopFrameEndpoints(this WebApplication app, string baseUrl = "api/v1/") {
        BaseUrl = baseUrl.Trim('/') + '/';
        Config = app.Services.GetRequiredService<HopFrameConfig>();
        
        app.ExtractIdentifiers(Config);
        
        foreach (var table in Config.Tables) {
            if (!TableConfiguratorExtensions.TableIdentifiers.ContainsKey(table.Identifier))
                throw new Exception($"No identifiers were configured for table '{table.DisplayName}'");
        }
        
        app.MapHopFrameEndpoint(HttpMethod.Get, "hopframe.json", () => Results.Json(Config));
        
        foreach (var table in Config.Tables) {
            app.MapHopFrameTable(table);
        }
        
        return app;
    }

    internal static RouteHandlerBuilder MapHopFrameEndpoint(this WebApplication app, HttpMethod method, [StringSyntax("Route")] string pattern, Delegate handler, string? claimOverride = null) {
        var route = BaseUrl + pattern.TrimStart('/');
        var claim = claimOverride ?? Config.BaseClaim;

        RouteHandlerBuilder builder = method.Method switch {
            "GET" => app.MapGet(route, handler),
            "PUT" => app.MapPut(route, handler),
            "POST" => app.MapPost(route, handler),
            "PATCH" => app.MapPatch(route, handler),
            "DELETE" => app.MapDelete(route, handler),
            _ => throw new ArgumentOutOfRangeException(nameof(method), method.Method, "Method not supported!")
        };

        if (Config.AllowAnonymousAccess)
            builder.AllowAnonymous();
        else if (!string.IsNullOrWhiteSpace(claim))
            builder.RequireAuthorization(auth => auth.RequireClaim(claim));
        else
            builder.RequireAuthorization();

        return builder;
    }
    
}
