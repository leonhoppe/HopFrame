using HopFrame.Core.Config;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Services.Implementations;

internal sealed class TableManager<TModel>(DbContext context, TableConfig config) : ITableManager where TModel : class {
    
    public IQueryable<object> LoadPage(int page, int perPage = 20) {
        var table = context.Set<TModel>();
        return table
            .Skip(page * perPage)
            .Take(perPage);
    }

    public (IEnumerable<object>, int) Search(string searchTerm, int page = 0, int perPage = 20) {
        var table = context.Set<TModel>();
        var all = table
            .AsEnumerable()
            .Where(item => ItemSearched(item, searchTerm))
            .ToList();

        return (all.Skip(page * perPage).Take(perPage), (int)Math.Ceiling(all.Count / (double)perPage));
    }

    public int TotalPages(int perPage = 20) {
        var table = context.Set<TModel>();
        return (int)Math.Ceiling(table.Count() / (double)perPage);
    }

    public async Task DeleteItem(object item) {
        var table = context.Set<TModel>();
        table.Remove((item as TModel)!);
        await context.SaveChangesAsync();
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
}