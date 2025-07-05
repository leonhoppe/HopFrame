using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HopFrame.Core.Services.Implementations;

internal sealed class TableManager<TModel>(DbContext context, TableConfig config, IContextExplorer explorer, IServiceProvider provider, ISearchExpressionBuilder searchExpressionBuilder) : ITableManager where TModel : class {
    
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

        var parameter = Expression.Parameter(typeof(TModel), "x");
        var exp = searchExpressionBuilder.BuildSearchExpression(config, searchTerm, parameter);
        
        if (exp is null)
            return ([], 0);
        
        var lambda = Expression.Lambda<Func<TModel, bool>>(exp, parameter);
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