using HopFrame.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Core.EFCore;

/// <summary>
/// The generic repository that handles data source communication for managed tables
/// </summary>
/// <typeparam name="TModel">The model that is managed by the repo</typeparam>
/// <typeparam name="TContext">The underlying context that handles database communication</typeparam>
public class EfCoreRepository<TModel, TContext>(TContext context) : HopFrameRepository<TModel> where TModel : class where TContext : DbContext {
    
    /// <inheritdoc/>
    public override Task<IEnumerable<TModel>> LoadPageAsync(int page, int perPage, CancellationToken ct = default) {
        throw new NotImplementedException(); //TODO: Implement loading functionality
    }
    
    /// <inheritdoc/>
    public override async Task<int> CountAsync(CancellationToken ct = default) {
        var table = context.Set<TModel>();
        return await table.CountAsync(ct);
    }
    
    /// <inheritdoc/>
    public override Task<IEnumerable<TModel>> SearchAsync(string searchTerm, int page, int perPage, CancellationToken ct = default) {
        throw new NotImplementedException(); //TODO: Implement search functionality
    }
    
    /// <inheritdoc/>
    public override async Task CreateAsync(TModel entry, CancellationToken ct = default) {
        await context.AddAsync(entry, ct);
        await context.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public override async Task UpdateAsync(TModel entry, CancellationToken ct = default) {
        context.Update(entry);
        await context.SaveChangesAsync(ct);
    }

    /// <inheritdoc/>
    public override async Task DeleteAsync(TModel entry, CancellationToken ct = default) {
        context.Remove(entry);
        await context.SaveChangesAsync(ct);
    }
    
}