using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HopFrame.Core.Services.Implementations;

internal sealed class TableManager<TModel>(DbContext context, TableConfig config, IContextExplorer explorer, IServiceProvider provider) : ITableManager where TModel : class {
    
    public async Task<IEnumerable<object>> LoadPage(int page, int perPage = 20) {
        var table = context.Set<TModel>();
        var data = IncludeForeignKeys(table);
        return await data
            .Skip(page * perPage)
            .Take(perPage)
            .ToArrayAsync();
    }

    public async Task<(IEnumerable<object>, int)> Search(string searchTerm, int page = 0, int perPage = 20) {
        var table = context.Set<TModel>();

        var isNegative = searchTerm.StartsWith('!');
        if (isNegative)
            searchTerm = searchTerm[1..];

        Type[] validTypes = [typeof(string), typeof(Guid)];
        
        var parameter = Expression.Parameter(typeof(TModel), "x");
        var properties = config.Properties
            .Where(prop => !prop.IsVirtualProperty)
            .Where(prop => prop.List)
            .Where(prop => prop.Info.PropertyType.IsEnum || validTypes.Contains(prop.Info.PropertyType))
            .Select(prop => prop.Info);

        Expression? combinedExpression = null;
        
        foreach (var property in properties) {
            var propertyAccess = Expression.Property(parameter, property);
            var toStringCall = Expression.Call(propertyAccess, nameof(ToString), Type.EmptyTypes);
            var searchExpression = Expression.Call(
                toStringCall,
                typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!,
                Expression.Constant(searchTerm));

            combinedExpression = combinedExpression == null
                ? searchExpression
                : Expression.OrElse(combinedExpression, searchExpression);
        }

        var foreignProperties = config.Properties
            .Where(prop => prop.List)
            .Where(prop => prop.IsRelation)
            .Where(prop => prop.DisplayedProperty != null)
            .Select(prop => (prop.Info, explorer
                .GetTable(prop.Info.PropertyType)!.Properties
                .Find(p => p.Info.Name == prop.DisplayedProperty!.Name)!
                .Info));
        
        foreach (var (navigationProperty, displayedProperty) in foreignProperties) {
            var navigationAccess = Expression.Property(parameter, navigationProperty);
            var nullCheck = Expression.NotEqual(navigationAccess, Expression.Constant(null));
            var displayedPropertyAccess = Expression.Property(navigationAccess, displayedProperty);

            var toStringCall = Expression.Call(displayedPropertyAccess, nameof(ToString), Type.EmptyTypes);
            var searchExpression = Expression.Call(
                toStringCall,
                typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!,
                Expression.Constant(searchTerm));

            var safeSearchExpression = Expression.AndAlso(nullCheck, searchExpression);

            combinedExpression = combinedExpression == null
                ? safeSearchExpression
                : Expression.OrElse(combinedExpression, safeSearchExpression);
        }

        if (combinedExpression == null)
            return (
                await LoadPage(page, perPage),
                await TotalPages(perPage));

        if (isNegative)
            combinedExpression = Expression.Not(combinedExpression);

        var lambda = Expression.Lambda<Func<TModel, bool>>(combinedExpression, parameter);
        var result = await IncludeForeignKeys(table)
            .Where(lambda)
            .Skip(page * perPage)
            .Take(perPage)
            .ToListAsync();

        var totalEntries = await table
            .Where(lambda)
            .CountAsync();

        return (result, (int)Math.Ceiling(totalEntries / (double)perPage));
    }

    public async Task<int> TotalPages(int perPage = 20) {
        var table = context.Set<TModel>();
        return (int)Math.Ceiling(await table.CountAsync() / (double)perPage);
    }

    public async Task DeleteItem(object item) {
        var table = context.Set<TModel>();
        table.Remove((item as TModel)!);
        await context.SaveChangesAsync();
    }

    public async Task EditItem(object item) {
        await context.SaveChangesAsync();
    }

    public async Task AddItem(object item) {
        var table = context.Set<TModel>();
        await table.AddAsync((TModel)item);
        await context.SaveChangesAsync();
    }

    public async Task AddAll(IEnumerable<object> items) {
        var table = context.Set<TModel>();
        await table.AddRangeAsync(items.Cast<TModel>());
        await context.SaveChangesAsync();
    }

    public async Task<object?> GetOne(object key) {
        var table = context.Set<TModel>();
        return await table.FindAsync(key);
    }

    public async Task<string> DisplayProperty(object? item, PropertyConfig prop, object? value = null, object? enumerableValue = null) {
        if (item is null) return string.Empty;

        if (prop.IsVirtualProperty)
            return await prop.Formatter!.Invoke(item, provider);

        var propValue = value ?? prop.GetValue(item, provider);
        if (propValue is null)
            return string.Empty;

        if (prop.Formatter is not null) {
            return await prop.Formatter.Invoke(propValue, provider);
        }

        if (prop.IsEnumerable) {
            if (enumerableValue is not null) {
                if (prop.EnumerableFormatter is not null) {
                    return await prop.EnumerableFormatter.Invoke(enumerableValue, provider);
                }
            
                return enumerableValue.ToString() ?? string.Empty;
            }
            
            return (propValue as IEnumerable)!.OfType<object>().Count().ToString();
        }

        if (prop.DisplayedProperty is null) {
            var key = prop.Info.PropertyType
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute));

            return key?.GetValue(propValue)?.ToString() ?? propValue.ToString() ?? string.Empty;
        }
        
        var innerConfig = explorer.GetTable(propValue.GetType());
        if (innerConfig is null) return propValue.ToString()!;
        
        var innerProp = innerConfig.Properties
            .SingleOrDefault(p => p.Info == prop.DisplayedProperty && !p.IsVirtualProperty);

        if (innerProp is null) return propValue.ToString() ?? string.Empty;
        return await DisplayProperty(propValue, innerProp);
    }

    private IQueryable<TModel> IncludeForeignKeys(IQueryable<TModel> query) {
        return config.Properties
            .Where(prop => prop.IsRelation)
            .Aggregate(query, (current, property) => current.Include(property.Info.Name));
    }

}