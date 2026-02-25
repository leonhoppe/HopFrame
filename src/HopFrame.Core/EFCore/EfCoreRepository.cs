using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.EFCore;

internal class EfCoreRepository<TModel, TContext>(TContext context, IConfigAccessor accessor) : HopFrameRepository<TModel> where TModel : class where TContext : DbContext {
    
    public override async Task<IEnumerable<TModel>> LoadPageAsync(int page, int perPage, CancellationToken ct = default) {
        var set = context.Set<TModel>();
        var query = set
            .AsNoTracking()
            .Skip(page * perPage)
            .Take(perPage);

        return await IncludeForeignKeys(query)
            .ToArrayAsync(ct);
    }

    private IQueryable<TModel> IncludeForeignKeys(IQueryable<TModel> query) {
        var table = accessor.GetTableByType(typeof(TModel))!;

        return table.Properties
            .Where(p => (p.PropertyType & PropertyType.Relation) != 0)
            .Aggregate(query, (current, property) => current.Include(property.Identifier));
    }
    
    public override async Task<int> CountAsync(CancellationToken ct = default) {
        var set = context.Set<TModel>();
        return await set.CountAsync(ct);
    }
    
    public override Task<IEnumerable<TModel>> SearchAsync(string searchTerm, int page, int perPage, CancellationToken ct = default) {
        return LoadPageAsync(page, perPage, ct); //TODO: Implement search functionality
    }
    
    public override async Task CreateAsync(TModel entry, CancellationToken ct = default) {
        await context.AddAsync(entry, ct);
        await context.SaveChangesAsync(ct);
    }

    public override async Task UpdateAsync(TModel entry, CancellationToken ct = default) {
        context.Update(entry);
        await context.SaveChangesAsync(ct);
    }

    public override async Task DeleteAsync(TModel entry, CancellationToken ct = default) {
        context.Remove(entry);
        await context.SaveChangesAsync(ct);
    }
    
}