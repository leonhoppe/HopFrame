using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using Microsoft.Extensions.Logging;

namespace HopFrame.Core.Services.Implementation;

internal sealed class SortService(IEntityAccessor entityAccessor, IConfigAccessor accessor, ILogger<SortService> logger) : ISortService {
    
    public IQueryable<TModel> Sort<TModel>(IQueryable<TModel> dataset, Sorting sorting, TableConfig table, bool executeIfUncompilable = false) where TModel : class {
        if (string.IsNullOrWhiteSpace(sorting.PropertyIdentifier))
            return dataset;

        var property = table.Properties.FirstOrDefault(p => p.Identifier == sorting.PropertyIdentifier);
        if (property is null)
            return dataset;
        
        return (IQueryable<TModel>)SortDataByProperty(dataset, property,
            sorting.Direction == ListSortDirection.Descending, executeIfUncompilable);
    }
    
    private IQueryable SortDataByProperty(IQueryable data, PropertyConfig property, bool descending, bool executeIfUncompilable) {
        var parameter = Expression.Parameter(property.Table.TableType);
        Expression expression;
        Type targetType = property.Type;

        var cantBeTranslated = false;

        if (property.Getter is not null) {
            var getterProp = typeof(PropertyConfig).GetProperty(nameof(PropertyConfig.Getter))!;
            var invokeMethod = typeof(Func<object, object?>).GetMethod(nameof(Func<>.Invoke))!;
            expression = Expression.Call(Expression.Property(Expression.Constant(property), getterProp), invokeMethod, Expression.Convert(parameter, typeof(object)));
            expression = Expression.Convert(expression, targetType);
            cantBeTranslated = true;
        }
        else {
            var prop = property.Table.TableType.GetProperty(property.Identifier);
            if (prop is null)
                return data;

            expression = Expression.Property(parameter, prop);

            if ((property.PropertyType & PropertyType.List) != 0) {
                var countPropInfo = property.Type.GetProperty(nameof(List<>.Count))!;
                expression = Expression.Property(expression, countPropInfo);
                targetType = typeof(int);
            }
            else if ((property.PropertyType & PropertyType.Relation) != 0) {
                var relationTable = accessor.GetTableByIdentifier(property.RelationTable!);
                PropertyInfo? relationPropInfo = null;
                
                if (relationTable?.PreferredProperty != null) {
                    var relationProp = relationTable.Properties.First(p => p.Identifier == relationTable.PreferredProperty);

                    if ((relationProp.PropertyType & PropertyType.List) == 0)
                        relationPropInfo = relationTable.TableType.GetProperty(relationProp.Identifier);
                }
            
                if (relationPropInfo == null) {
                    var formatMethod = entityAccessor.GetType().GetMethod(nameof(IEntityAccessor.FormatValue))!;
                    targetType = typeof(string);
                    expression = Expression.Call(Expression.Constant(entityAccessor), formatMethod, expression, Expression.Constant(property), Expression.Constant(false));
                    cantBeTranslated = true;
                }
                else {
                    targetType = relationPropInfo.PropertyType;
                    expression = Expression.Property(expression, relationPropInfo);
                }
            }
        }
        
        var lambda = Expression.Lambda(expression, parameter);

        var methodName = descending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);
        var method = typeof(Queryable)
            .GetMethods()
            .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(property.Table.TableType, targetType);

        if (cantBeTranslated && !executeIfUncompilable) {
            logger.LogWarning("Cannot automatically sort table {table} by computed property {property}", property.Table.Identifier, property.Identifier);
            return data;
        }
        
        var result = method.Invoke(null, [data, lambda]);
        return (IQueryable)result!;
    }
    
}