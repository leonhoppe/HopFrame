using System.Linq.Expressions;
using HopFrame.Core.Configuration;

namespace HopFrame.Core.Services.Implementation;

internal sealed class SearchService(IConfigAccessor accessor) : ISearchService {
    
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

        if ((property.PropertyType & PropertyType.Relation) != 0 && property.RelationType != null) {
            var relationTable = accessor.GetTableByIdentifier(property.RelationTable!);
            if (relationTable is null)
                return null;
            
            var preferred = relationTable.PreferredProperty;
            if (preferred is null)
                return null;

            var relProp = property.RelationType.GetProperty(preferred);
            if (relProp is null)
                return null;

            expr = Expression.Property(expr, relProp);
        }

        return expr;
    }
    
}