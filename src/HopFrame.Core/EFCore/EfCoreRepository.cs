using HopFrame.Core.Configuration;
using HopFrame.Core.Repositories;
using HopFrame.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HopFrame.Core.EFCore;

internal class EfCoreRepository<TModel, TContext>(TContext context, IEntityAccessor entityAccessor, ISearchService searchService, ISortService sortService, IConfigAccessor configAccessor, ILogger<EfCoreRepository<TModel, TContext>> logger) : HopFrameRepository<TModel> where TModel : class where TContext : DbContext {

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
        var entityType = context.Model.FindEntityType(typeof(TModel));
        var key = entityType!.FindPrimaryKey();

        if (key is null) {
            logger.LogCritical("In order to update an entry of table {name}, the model cannot be keyless!", _table.DisplayName);
            return;
        }

        var keyValues = key.Properties
            .Select(p => p.PropertyInfo!.GetValue(entry))
            .ToArray();

        var existing = await GetTrackedEntryAsync(keyValues, ct);
        if (existing is null) {
            logger.LogError("An existing entry with for {name} with the keys [{}] could not be found. Entry updating will be aboarded", _table.DisplayName, string.Join(", ", keyValues));
            return;
        }

        var tracker = context.Entry(existing);
        tracker.CurrentValues.SetValues(entry);

        foreach (var navigation in entityType.GetNavigations()) {
            if (navigation.IsCollection) {
                var currentCollection = (IList?)navigation.PropertyInfo!.GetValue(existing);
                var newCollection = (IList?)navigation.PropertyInfo!.GetValue(entry);

                var currProperty = _table.Properties.First(p => p.Identifier == navigation.PropertyInfo!.Name && p.RelationTable is not null);
                var otherRepo = configAccessor.LoadRepository(configAccessor.GetTableByIdentifier(currProperty.RelationTable!)!);
                var equalMethod = otherRepo.GetType().GetMethod(nameof(KeysEqual));
                if (equalMethod is null) {
                    logger.LogError("Cannot transfer collection because relation type '{name}' is not a ef core repository!", currProperty.Type);
                    continue;
                }

                if (currentCollection is null) {
                    currentCollection = (IList)Activator.CreateInstance(navigation.ClrType)!;
                    navigation.PropertyInfo!.SetValue(existing, currentCollection);
                }

                if (newCollection is null) {
                    continue;
                }

                var entriesForRemoal = new List<object>();
                var entriesForAdding = new List<object>();

                foreach (var current in currentCollection) {
                    var found = false;

                    foreach (var newEntry in newCollection) {
                        if ((bool)equalMethod.Invoke(otherRepo, [current, newEntry])!) {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        entriesForRemoal.Add(current);
                }

                foreach (var item in newCollection) {
                    var found = false;

                    foreach (var currentEntry in currentCollection) {
                        if ((bool)equalMethod.Invoke(otherRepo, [item, currentEntry])!) {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        entriesForAdding.Add(item);
                }

                foreach (var e in entriesForRemoal) {
                    currentCollection.Remove(e);
                }
                foreach (var e in entriesForAdding) {
                    currentCollection.Add(e);
                }
            }

            var property = navigation.PropertyInfo;

            if (property is null)
                continue;

            var newValue = property.GetValue(entry);

            property.SetValue(existing, newValue);
        }

        await context.SaveChangesAsync(ct);
        context.ChangeTracker.Clear();
    }

    public override async Task DeleteAsync(TModel entry, CancellationToken ct = default) {
        context.Remove(entry);
        await context.SaveChangesAsync(ct);
    }

    public async Task<TModel?> GetTrackedEntryAsync(object?[] keyValues, CancellationToken ct) {
        var entityType = context.Model.FindEntityType(typeof(TModel));
        var key = entityType!.FindPrimaryKey();

        if (key is null) {
            logger.LogCritical("In order to update an entry of table {name}, the model cannot be keyless!", _table.DisplayName);
            return null;
        }

        var set = context.Set<TModel>();
        var query = IncludeForeignKeys(set);

        var parameter = Expression.Parameter(typeof(TModel), "e");
        var body = key.Properties
            .Select((p, i) =>
                Expression.Equal(
                    Expression.Property(parameter, p.Name),
                    Expression.Constant(keyValues[i], p.ClrType)))
            .Aggregate(Expression.AndAlso);

        var predicate = Expression.Lambda<Func<TModel, bool>>(body, parameter);
        return await query.FirstOrDefaultAsync(predicate, ct);
    }

    public bool KeysEqual(TModel x, TModel y) {
        var entityType = context.Model.FindEntityType(typeof(TModel));
        var key = entityType!.FindPrimaryKey();

        if (key is null) {
            logger.LogCritical("In order to update an entry of table {name}, the model cannot be keyless!", _table.DisplayName);
            return false;
        }

        var xParameter = Expression.Parameter(typeof(TModel), "a");
        var yParameter = Expression.Parameter(typeof(TModel), "b");
        var body = key.Properties
            .Select((p, i) =>
                Expression.Equal(
                    Expression.Property(xParameter, p.Name),
                    Expression.Property(yParameter, p.Name)))
            .Aggregate(Expression.AndAlso);

        var exp = Expression.Lambda<Func<TModel, TModel, bool>>(body, xParameter, yParameter).Compile();
        return exp(x, y);
    }
    
}