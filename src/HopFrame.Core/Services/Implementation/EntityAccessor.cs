using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Core.Configuration;

namespace HopFrame.Core.Services.Implementation;

internal class EntityAccessor(IConfigAccessor accessor) : IEntityAccessor {
    
    public string? GetValue(object model, PropertyConfig property) {
        var value = GetValueRaw(model, property);
        if (value is null)
            return null;
        
        return FormatValue(value, property);
    }

    public object? GetValueRaw(object model, PropertyConfig property) {
        if (property.Getter is not null)
            return property.Getter.Invoke(model);
        
        var prop = model.GetType().GetProperty(property.Identifier);
        if (prop is null)
            return null;

        return prop.GetValue(model);
    }

    public string? FormatValue(object? value, PropertyConfig property) {
        if (value is null)
            return null;
        
        if ((property.PropertyType & PropertyType.List) != 0) {
            return (value as IEnumerable<object>)!.Count().ToString();
        }

        if ((property.PropertyType & PropertyType.Relation) != 0) {
            var table = accessor.GetTableByType(property.Type);
            if (table?.PreferredProperty != null) {
                var tableProp = table.Properties.First(p => p.Identifier == table.PreferredProperty);
                return GetValue(value, tableProp);
            }
        }

        return value.ToString();
    }

    public void SetValue(object model, PropertyConfig property, object? value) {
        if (property.Setter is not null) {
            property.Setter.Invoke(model, value);
            return;
        }
        
        var prop = model.GetType().GetProperty(property.Identifier);
        if (prop is null)
            return;

        if (value?.GetType() != property.Type)
            value = Convert.ChangeType(value, property.Type);
        
        prop.SetValue(model, value);
    }

    public IEnumerable<object> SortDataByProperty(IEnumerable<object> data, PropertyConfig property, bool descending = false) {
        var parameter = Expression.Parameter(property.Table.TableType);
        Expression expression;
        Type targetType = property.Type;

        if (property.Getter is not null) {
            var getterProp = typeof(PropertyConfig).GetProperty(nameof(PropertyConfig.Getter))!;
            var invokeMethod = typeof(Func<object, object?>).GetMethod(nameof(Func<>.Invoke))!;
            expression = Expression.Call(Expression.Property(Expression.Constant(property), getterProp), invokeMethod, Expression.Convert(parameter, typeof(object)));
            expression = Expression.Convert(expression, targetType);
        }
        else {
            var prop = property.Table.TableType.GetProperty(property.Identifier);
            if (prop is null)
                return data;

            expression = Expression.Property(parameter, prop);

            if ((property.PropertyType & PropertyType.Relation) != 0) {
                var relationTable = accessor.GetTableByType(property.Type);
                PropertyInfo? relationPropInfo = null;

                if (relationTable?.PreferredProperty != null) {
                    var relationProp = relationTable.Properties.First(p => p.Identifier == relationTable.PreferredProperty);
                
                    if ((relationProp.PropertyType & PropertyType.List) == 0)
                        relationPropInfo = relationProp.Type.GetProperty(relationProp.Identifier);
                }
            
                if (relationPropInfo == null) {
                    var formatMethod = GetType().GetMethod(nameof(FormatValue))!;
                    targetType = typeof(string);
                    expression = Expression.Call(Expression.Constant(this), formatMethod, expression, Expression.Constant(property));
                }
                else {
                    targetType = relationPropInfo.PropertyType;
                    expression = Expression.Property(expression, relationPropInfo);
                }
            }
        }
        
        var lambda = Expression.Lambda(expression, parameter);

        var methodName = descending ? nameof(Enumerable.OrderByDescending) : nameof(Enumerable.OrderBy);
        var method = typeof(Enumerable)
            .GetMethods()
            .Single(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(property.Table.TableType, targetType);

        var result = method.Invoke(null, [data, lambda.Compile()]);
        return (IEnumerable<object>)result!;
    }
    
}