using System.Collections;
using System.ComponentModel.DataAnnotations;
using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Services.Implementations;

internal sealed class TableManager<TModel>(DbContext context, TableConfig config, IContextExplorer explorer, IServiceProvider provider) : ITableManager where TModel : class {
    
    public IQueryable<object> LoadPage(int page, int perPage = 20) {
        var table = context.Set<TModel>();
        var data = IncludeForeignKeys(table);
        return data
            .Skip(page * perPage)
            .Take(perPage);
    }

    public Task<(IEnumerable<object>, int)> Search(string searchTerm, int page = 0, int perPage = 20) {
        var table = context.Set<TModel>();
        var all = IncludeForeignKeys(table)
            .AsEnumerable()
            .Where(item => ItemSearched(item, searchTerm))
            .ToList();

        return Task.FromResult((
            (IEnumerable<object>)all.Skip(page * perPage).Take(perPage),
            (int)Math.Ceiling(all.Count / (double)perPage)));
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

    public async Task RevertChanges(object item) {
        await context.Entry((TModel)item).ReloadAsync();
    }

    private bool ItemSearched(TModel item, string searchTerm) {
        foreach (var property in config.Properties) {
            if (!property.Searchable) continue;
            var value = property.Info.GetValue(item);
            if (value is null) continue;
            
            var strValue = value.ToString();
            if (strValue?.Contains(searchTerm) == true) 
                return true;
        }

        return false;
    }

    public string DisplayProperty(object? item, PropertyConfig prop, object? value = null) {
        if (item is null) return string.Empty;

        if (prop.IsListingProperty)
            return prop.Formatter!.Invoke(item, provider);

        var propValue = value ?? prop.Info.GetValue(item);
        if (propValue is null)
            return string.Empty;

        if (prop.Formatter is not null) {
            return prop.Formatter.Invoke(propValue, provider);
        }

        if (prop.IsEnumerable) {
            if (value is not null) {
                if (prop.EnumerableFormatter is not null) {
                    return prop.EnumerableFormatter.Invoke(value, provider);
                }
            
                return value.ToString() ?? string.Empty;
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
        
        var innerProp = innerConfig!.Properties
            .SingleOrDefault(p => p.Info == prop.DisplayedProperty && !p.IsListingProperty);

        if (innerProp is null) return propValue.ToString() ?? string.Empty;
        return DisplayProperty(propValue, innerProp);
    }

    private IQueryable<TModel> IncludeForeignKeys(IQueryable<TModel> query) {
        return config.Properties
            .Where(prop => prop.IsRelation)
            .Aggregate(query, (current, property) => current.Include(property.Info.Name));
    }

}