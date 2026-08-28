using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using HopFrame.Core.Configuration;
using HopFrame.Core.Helpers;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

namespace HopFrame.API.Endpoints;

internal static class EndpointBuilder {
    
    public class HopFramePageResult<T> {
        public int Pages { get; set; }
        public IEnumerable<T> Data { get; set; } = [];
    }
    
    private static readonly JsonSerializerOptions JsonOptions = new() {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        WriteIndented = false
    };

    public static void MapHopFrameTable(this WebApplication app, TableConfig config) {
        var keys = TableConfiguratorExtensions.TableIdentifiers[config.Identifier].ToArray();
        var pagedResultType = typeof(HopFramePageResult<>).MakeGenericType(config.TableType);
        
        async Task<object> GetEntryFromKeys(HttpContext context, IHopFrameRepository repo, CancellationToken ct) {
            if (keys.Any(k => !context.Request.Query.ContainsKey(k)))
                return Results.Problem("Not all identifying keys are present", statusCode: StatusCodes.Status400BadRequest);
            
            var keyValues = keys.Select(k => context.Request.Query[k].ToString()).ToArray();
            object[] parsedKeys;

            try {
                parsedKeys = ParseKeys(keys, keyValues, config);
            }
            catch (ArgumentOutOfRangeException e) {
                return Results.ValidationProblem([
                    new(e.ParamName!,
                        ["Provided value could not be converted to proper key type"])
                ]);
            }

            var entry = await repo.GetUniqueEntryGenericAsync(parsedKeys, ct);
            if (entry is null)
                return Results.Problem("The desired entry could not be found", statusCode: StatusCodes.Status404NotFound);

            return entry;
        }
        
        app.MapHopFrameEndpoint(HttpMethod.Get, config.Route, async (IConfigAccessor accessor, CancellationToken ct,
            [FromQuery] int page = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string? sortRow = null,
            [FromQuery] string sortDir = "asc",
            [FromQuery] string? search = null) =>
        {
            var repo = accessor.LoadRepository(config);
            var sorting = new Sorting(sortRow, sortDir == "asc" ? ListSortDirection.Ascending : ListSortDirection.Descending);

            IEnumerable<object> entries;
            int totalCount;

            if (string.IsNullOrWhiteSpace(search)) {
                entries = await repo.LoadPageGenericAsync(page, perPage, sorting, ct);
                totalCount = (int)Math.Ceiling((double)await repo.CountAsync(ct) / perPage);
            }
            else {
                var result = await repo.SearchGenericAsync(search, page, perPage, sorting, ct);
                entries = result.Result;
                totalCount = result.PageCount;
            }
            
            return Results.Json(new {pages = totalCount, data = entries}, JsonOptions);
        }, config.ViewClaim)
        .WithTags(config.DisplayName)
        .Produces(StatusCodes.Status200OK, responseType: pagedResultType);
        
        app.MapHopFrameEndpoint(HttpMethod.Delete, config.Route, async (IConfigAccessor accessor, CancellationToken ct, HttpContext context) => {
            var repo = accessor.LoadRepository(config);
            var entry = await GetEntryFromKeys(context, repo, ct);
            if (entry is IResult result)
                return result;

            await repo.DeleteGenericAsync(entry, ct);
            return Results.Ok();
        }, config.EditClaim)
        .WithTags(config.DisplayName)
        .Produces(StatusCodes.Status200OK)
        .AppendKeyMetadata(keys, config);
    }

    private static object[] ParseKeys(string[] keyIdentifiers, string[] keyValues, TableConfig table) {
        object[] parsed = new object[keyIdentifiers.Length];
        
        for (var i = 0; i < keyIdentifiers.Length; i++) {
            var property = table.Properties.First(p => p.Identifier == keyIdentifiers[i]);
            var value = keyValues[i];

            if (property.Type == typeof(Guid)) {
                if (!Guid.TryParse(value, out var guid)) {
                    throw new ArgumentOutOfRangeException(keyIdentifiers[i]);
                }
                
                parsed[i] = guid;
                continue;
            }

            try {
                parsed[i] = Convert.ChangeType(value, property.Type);
            }
            catch {
                throw new ArgumentOutOfRangeException(keyIdentifiers[i]);
            }
        }

        return parsed;
    }

    private static RouteHandlerBuilder AppendKeyMetadata(this RouteHandlerBuilder builder, IEnumerable<string> keys, TableConfig table) {
        return builder
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .AddOpenApiOperationTransformer((op, _, _) => {
                op.Parameters ??= new List<IOpenApiParameter>();

                foreach (var keyName in keys) {
                    var prop = table.Properties.FirstOrDefault(p => p.Identifier == keyName);

                    op.Parameters.Add(new OpenApiParameter {
                        Name = keyName,
                        In = ParameterLocation.Query,
                        Required = true,
                        Schema = new OpenApiSchema {
                            Type = prop?.Type.IsNumeric() == true ? JsonSchemaType.Number : JsonSchemaType.String
                        }
                    });
                }

                return Task.CompletedTask;
            });
    }
    
}