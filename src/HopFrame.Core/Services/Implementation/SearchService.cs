using System.Collections;
using System.Linq.Expressions;
using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace HopFrame.Core.Services.Implementation;

internal sealed class SearchService(IConfigAccessor accessor, ILogger<SearchService> logger) : ISearchService {

    public IQueryable<TModel> Search<TModel>(IQueryable<TModel> dataset, TableConfig table, string searchTerm) where TModel : notnull {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return dataset;

        searchTerm = searchTerm.Trim().ToLower();

        var parameter = Expression.Parameter(typeof(TModel), "x");
        Expression? combined = null;

        foreach (var property in table.Properties.Where(p => p.Searchable)) {
            if (property.Getter is not null)
                continue;

            Expression? valueExpr = BuildPropertyAccess(parameter, property);
            if (valueExpr == null)
                continue;
            
            if (property.Type != typeof(string))
                valueExpr = Expression.Call(valueExpr, nameof(ToString), Type.EmptyTypes);
            
            var toLower = Expression.Call(valueExpr, nameof(string.ToLower), Type.EmptyTypes);
            var contains = Expression.Call(toLower, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(searchTerm));

            combined = combined == null
                ? contains
                : Expression.OrElse(combined, contains);
        }

        if (combined == null)
            return dataset;

        logger.LogDebug("Generated search expression: {expression}", combined);

        var lambda = Expression.Lambda<Func<TModel, bool>>(combined, parameter);
        return dataset.Where(lambda);
    }

    public IQueryable<TModel> Search<TModel>(IQueryable<TModel> dataset, TableConfig table, IEnumerable<AdvancedSearchProperty> properties) where TModel : notnull {
        var parameter = Expression.Parameter(typeof(TModel), "x");
        Expression? combined = null;

        foreach (var searchTerm in properties) {
            var property = table.Properties.First(p => p.Identifier == searchTerm.Identifier);
            Expression valueExp = Expression.Property(parameter, property.Identifier);

            if (property.PropertyType.HasFlag(PropertyType.List)) {
                valueExp = Expression.Property(valueExp, nameof(IList.Count));
            }

            if (valueExp is null) continue;

            if (searchTerm.Equal is not null) {
                if (property.PropertyType.HasFlag(PropertyType.Relation) && !property.PropertyType.HasFlag(PropertyType.List)) {
                    var relationTable = accessor.GetTableByIdentifier(property.RelationTable!)!;
                    if (relationTable.KeyProperties.Length == 0 && relationTable.PreferredProperty is null) {
                        logger.LogError("In order to filter for relations of type '{type}' at least one primary key needs to be present", property.Type.Name);
                        continue;
                    }

                    var keyProperties = relationTable.KeyProperties.Length == 0 ? [relationTable.PreferredProperty!] : relationTable.KeyProperties;
                    Expression? keyExpression = null!;
                    foreach (var keyProp in keyProperties) {
                        var otherAccess = Expression.Property(valueExp, keyProp);
                        var searchAccess = Expression.Property(Expression.Constant(searchTerm.Equal), keyProp);
                        var keyExp = Expression.Equal(otherAccess, searchAccess);

                        keyExpression = keyExpression is null ?
                            keyExp :
                            Expression.AndAlso(keyExpression, keyExp);
                    }

                    valueExp = keyExpression;
                }
                else if (property.Type == typeof(string) && !searchTerm.Exact) {
                    valueExp = Expression.Call(valueExp, nameof(string.ToLower), Type.EmptyTypes);
                    valueExp = Expression.Call(valueExp, nameof(string.Contains), Type.EmptyTypes, Expression.Constant(searchTerm.Equal.ToString()!.ToLower()));
                }
                else {
                    valueExp = Expression.Equal(valueExp, Expression.Constant(searchTerm.Equal));
                }
            } else {
                if (searchTerm.LessThan is not null && searchTerm.MoreThan is not null) {
                    valueExp = Expression.AndAlso(
                        Expression.LessThan(valueExp, Expression.Constant(searchTerm.LessThan)),
                        Expression.GreaterThan(valueExp, Expression.Constant(searchTerm.MoreThan)));
                }
                else if (searchTerm.LessThan is not null) {
                    valueExp = Expression.LessThan(valueExp, Expression.Constant(searchTerm.LessThan));
                }
                else if (searchTerm.MoreThan is not null) {
                    valueExp = Expression.GreaterThan(valueExp, Expression.Constant(searchTerm.MoreThan));
                }
            }

            combined = combined is null ?
                valueExp :
                Expression.AndAlso(combined, valueExp);
        }

        if (combined is null)
            return dataset;

        logger.LogDebug("Generated search expression: {expression}", combined);

        var lambda = Expression.Lambda<Func<TModel, bool>>(combined, parameter);
        return dataset.Where(lambda);
    }

    private Expression? BuildPropertyAccess(ParameterExpression parameter, PropertyConfig property) {
        if ((property.PropertyType & PropertyType.List) != 0)
            return null;
        
        var propInfo = property.Table.TableType.GetProperty(property.Identifier);
        if (propInfo == null)
            return null;

        Expression expr = Expression.Property(parameter, propInfo);

        if ((property.PropertyType & PropertyType.Relation) != 0 && property.RelationTable != null) {
            var relationTable = accessor.GetTableByIdentifier(property.RelationTable!);
            if (relationTable is null)
                return null;
            
            var preferred = relationTable.PreferredProperty;
            if (preferred is null)
                return null;

            var relProp = relationTable.TableType.GetProperty(preferred);
            if (relProp is null)
                return null;

            expr = Expression.Property(expr, relProp);
        }

        return expr;
    }
    
}