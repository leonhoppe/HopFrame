using System.Linq.Expressions;
using System.Reflection;
using HopFrame.Core.Config;

namespace HopFrame.Core.Services.Implementations;

internal sealed class SearchExpressionBuilder(IContextExplorer explorer) : ISearchExpressionBuilder {
    private readonly struct SearchPart {
        public string? Property { get; init; }
        public string Term { get; init; }
        public bool Negated { get; init; }
    }
    
    private Expression AddPropertySearchExpression(PropertyInfo property, ParameterExpression parameter, string searchTerm, PropertyConfig config) {
        Expression propertyAccess = Expression.Property(parameter, property);

        if (config.IsEnumerable) { //Call Count() extension method before checking the search term
            propertyAccess = Expression.Property(propertyAccess, config.Info.PropertyType.GetProperty(nameof(List<object>.Count))!);
        }
        
        var toStringCall = Expression.Call(propertyAccess, nameof(ToString), Type.EmptyTypes);
        var searchExpression = Expression.Call(
            toStringCall,
            typeof(string).GetMethod(config.IsEnumerable ? nameof(string.Equals) : nameof(string.Contains), [typeof(string)])!,
            Expression.Constant(searchTerm));

        return searchExpression;
    }

    private Expression AddForeignPropertySearchExpression(PropertyInfo navigationProperty, PropertyInfo displayedProperty, ParameterExpression parameter, string searchTerm) {
        var navigationAccess = Expression.Property(parameter, navigationProperty);
        var nullCheck = Expression.NotEqual(navigationAccess, Expression.Constant(null));
        var displayedPropertyAccess = Expression.Property(navigationAccess, displayedProperty);

        var toStringCall = Expression.Call(displayedPropertyAccess, nameof(ToString), Type.EmptyTypes);
        var searchExpression = Expression.Call(
            toStringCall,
            typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!,
            Expression.Constant(searchTerm));

        return Expression.AndAlso(nullCheck, searchExpression);
    }

    private IEnumerable<PropertyInfo> GetSuitableProperties(TableConfig table) {
        Type[] validTypes = [typeof(string), typeof(Guid), typeof(DateTime), typeof(DateOnly), typeof(TimeOnly)];
        
        return table.Properties
            .Where(prop => !prop.IsVirtualProperty)
            .Where(prop => prop.List)
            .Where(prop => prop.Searchable)
            .Where(prop => prop.Info.PropertyType.IsEnum || validTypes.Contains(prop.Info.PropertyType) || prop.IsEnumerable)
            .Select(prop => prop.Info);
    }

    private IEnumerable<(PropertyInfo navigation, PropertyInfo display)> GetSuitableForeignProperties(TableConfig table) {
        return table.Properties
            .Where(prop => prop.List)
            .Where(prop => prop.IsRelation)
            .Where(prop => prop.Searchable)
            .Where(prop => prop.DisplayedProperty != null)
            .Select(prop => (prop.Info, explorer
                .GetTable(prop.Info.PropertyType)!.Properties
                .Find(p => p.Info.Name == prop.DisplayedProperty!.Name)!
                .Info));
    }

    private IEnumerable<SearchPart> ExtractSearchParts(string searchTerm) {
        var rawParts = searchTerm.Split(' ');
        var parts = new List<SearchPart>();
        
        foreach (var part in rawParts) {
            if (string.IsNullOrWhiteSpace(part))
                continue;
            
            if (!part.Contains('=')) {
                var negated = part.StartsWith('!');
                
                parts.Add(new() {
                    Term = negated ? part[1..] : part,
                    Negated = negated,
                });
                continue;
            }

            var split = part.Split('=');
            var term = string.Join('=', split[1..]);
            var termNegated = term.StartsWith('!');
            
            parts.Add(new() {
                Property = split[0],
                Term = termNegated ? term[1..] : term,
                Negated = termNegated
            });
        }

        return parts;
    }

    public Expression? BuildSearchExpression(TableConfig table, string searchTerm, ParameterExpression parameter) {
        var properties = GetSuitableProperties(table).ToArray();
        var foreignProperties = GetSuitableForeignProperties(table).ToArray();

        var parts = ExtractSearchParts(searchTerm);
        
        Expression? expression = null;
        foreach (var part in parts) {
            Expression? subExp = null;
            
            if (part.Property is null) {
                foreach (var property in properties) {
                    var exp = AddPropertySearchExpression(property, parameter, part.Term, table.Properties.First(p => p.Info == property));
                    subExp = subExp is null
                        ? exp
                        : Expression.OrElse(subExp, exp);
                }
                
                foreach (var property in foreignProperties) {
                    var exp = AddForeignPropertySearchExpression(property.navigation, property.display, parameter, part.Term);
                    subExp = subExp is null
                        ? exp
                        : Expression.OrElse(subExp, exp);
                }
                
                if (subExp is null)
                    continue;
            }

            var prop = properties.FirstOrDefault(p => p.Name == part.Property);
            if (prop is not null) {
                subExp = AddPropertySearchExpression(prop, parameter, part.Term, table.Properties.First(p => p.Info == prop));
            }

            var forProp = foreignProperties.FirstOrDefault(p => p.navigation.Name == part.Property);
            if (forProp.navigation is not null) {
                subExp = AddForeignPropertySearchExpression(forProp.navigation, forProp.display, parameter, part.Term);
            }
            
            if (subExp is null)
                continue;
            
            if (part.Negated)
                subExp = Expression.Not(subExp);

            expression = expression is null
                ? subExp
                : Expression.AndAlso(expression, subExp);
        }

        return expression;
    }
}