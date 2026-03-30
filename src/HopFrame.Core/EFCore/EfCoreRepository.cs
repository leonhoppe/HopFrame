using System.Collections;
using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace HopFrame.Core.EFCore;

internal class EfCoreRepository<TModel, TContext>(TContext context, IEntityAccessor entityAccessor, ISearchService searchService, ISortService sortService, ILogger<EfCoreRepository<TModel, TContext>> logger) : HopFrameRepository<TModel> where TModel : class where TContext : DbContext {

    private TableConfig _table = null!;

    public override void Initialize(TableConfig table) {
        _table = table;
    }

    public override async Task<IEnumerable<TModel>> LoadPageAsync(int page, int perPage, Sorting sorting, CancellationToken ct = default) {
        try {
            var set = context.Set<TModel>();
            var query = set
                .AsNoTracking();

            query = sortService.Sort(query, sorting, _table)
                .Skip(page * perPage)
                .Take(perPage);

            return await IncludeForeignKeys(query)
                .ToArrayAsync(ct);
        }
        catch (Exception e) {
            logger.LogError(e, "Cannot load table {name}", _table.Identifier);
            return Enumerable.Empty<TModel>();
        }
    }

    private IQueryable<TModel> IncludeForeignKeys(IQueryable<TModel> query) {
        return _table.Properties
            .Where(p => (p.PropertyType & PropertyType.Relation) != 0)
            .Aggregate(query, (current, property) => current.Include(property.Identifier));
    }
    
    public override async Task<int> CountAsync(CancellationToken ct = default) {
        var set = context.Set<TModel>();
        return await set.CountAsync(ct);
    }
    
    public override async Task<SearchResult> SearchAsync(string searchTerm, int page, int perPage, Sorting sorting, CancellationToken ct = default) {
        try {
            var set = context.Set<TModel>();
            var query = set
                .AsNoTracking();

            query = IncludeForeignKeys(query);
            query = searchService.Search(query, _table, searchTerm);
            query = sortService.Sort(query, sorting, _table);

            var count = await query.CountAsync(ct);
            var data = await query
                .Skip(page * perPage)
                .Take(perPage)
                .ToArrayAsync(ct);

            return new(data, count);
        }
        catch (Exception e) {
            logger.LogError(e, "Cannot load table {name}", _table.Identifier);
            return new(Enumerable.Empty<TModel>(), 1);
        }
    }
    
    public override async Task CreateAsync(TModel entry, CancellationToken ct = default) {
        var relationTrackers = new List<EntityEntry>();
        foreach (var relation in _table.Properties.Where(p => (p.PropertyType & PropertyType.Relation) != 0)) {
            var value = entityAccessor.GetValueRaw(entry, relation);
            if (value is not null) {
                if ((relation.PropertyType & PropertyType.List) != 0) {
                    foreach (var obj in (IList)value) {
                        relationTrackers.Add(context.Attach(obj));
                    }
                }
                else {
                    relationTrackers.Add(context.Attach(value));
                }
            }
        }
        
        var tracker = await context.AddAsync(entry, ct);
        await context.SaveChangesAsync(ct);
        tracker.State = EntityState.Detached;
        relationTrackers.ForEach(t => t.State = EntityState.Detached);
    }

    public override async Task UpdateAsync(TModel entry, CancellationToken ct = default) {
        var tracker = context.Update(entry);
        await context.SaveChangesAsync(ct);
        tracker.State = EntityState.Detached;
    }

    public override async Task DeleteAsync(TModel entry, CancellationToken ct = default) {
        context.Remove(entry);
        await context.SaveChangesAsync(ct);
    }
    
}