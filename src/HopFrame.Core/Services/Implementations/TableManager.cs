using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.Services.Implementations;

internal sealed class TableManager<TModel>(DbContext context) : ITableManager where TModel : class {
    
    public async Task<IEnumerable<object>> LoadPage(int page, int perPage = 25) {
        var table = context.Set<TModel>();
        return await table
            .Skip(page * perPage)
            .Take(perPage)
            .ToArrayAsync();
    }
    
}