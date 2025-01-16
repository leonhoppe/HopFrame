using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Services.Implementations;

internal sealed class TableManager<TModel>(DbContext context, TableConfig config, IContextExplorer explorer) : ITableManager where TModel : class {
    
    public IQueryable<object> LoadPage(int page, int perPage = 20) {
        var table = context.Set<TModel>();
        var data = IncludeForgeinKeys(table);
        return data
            .Skip(page * perPage)
            .Take(perPage);
    }

    public Task<(IEnumerable<object>, int)> Search(string searchTerm, int page = 0, int perPage = 20) {
        var table = context.Set<TModel>();
        var all = IncludeForgeinKeys(table)
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

    public string DisplayProperty(object? item, PropertyInfo info, TableConfig? tableConfig) {
        if (item is null) return string.Empty;
        var prop = tableConfig?.Properties.Find(prop => prop.Info.Name == info.Name);
        if (prop is null) return item.ToString() ?? string.Empty;

        var propValue = prop.Info.GetValue(item);
        if (propValue is null)
            return string.Empty;

        if (prop.Formatter is not null) {
            return prop.Formatter.Invoke(propValue);
        }

        if (prop.DisplayedProperty is null) {
            var key = prop.Info.PropertyType
                .GetProperties()
                .Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute))
                .FirstOrDefault();

            return key?.GetValue(propValue)?.ToString() ?? propValue.ToString() ?? string.Empty;
        }
        
        var innerConfig = explorer.GetTable(propValue.GetType());
        return DisplayProperty(propValue, prop.DisplayedProperty, innerConfig);
    }

    private IQueryable<TModel> IncludeForgeinKeys(IQueryable<TModel> query) {
        var pendingQuery = query;
        
        foreach (var property in config.Properties) {
            var attr = property.Info
            .GetCustomAttributes(true)
            .FirstOrDefault(att => att is ForeignKeyAttribute) as ForeignKeyAttribute;

            if (attr is null) continue;

            pendingQuery = pendingQuery.Include(property.Info.Name);
        }

        return pendingQuery;
    }

}