using HopFrame.Core.Configuration;

namespace HopFrame.Core.Repositories;

/// The base repository that provides access to the model dataset
public abstract class HopFrameRepository<TModel> : IHopFrameRepository where TModel : class {

    /// <inheritdoc cref="LoadPageGenericAsync"/>
    public abstract Task<IEnumerable<TModel>> LoadPageAsync(int page, int perPage, Sorting sorting, CancellationToken ct = default);
    
    /// <inheritdoc/>
    public abstract Task<int> CountAsync(CancellationToken ct = default);
    
    /// <inheritdoc cref="SearchGenericAsync"/>
    public abstract Task<SearchResult> SearchAsync(string searchTerm, int page, int perPage, Sorting sorting, CancellationToken ct = default);

    /// <inheritdoc cref="CreateGenericAsync"/>
    public abstract Task CreateAsync(TModel entry, CancellationToken ct = default);
    
    /// <inheritdoc cref="UpdateGenericAsync"/>
    public abstract Task UpdateAsync(TModel entry, CancellationToken ct = default);
    
    /// <inheritdoc cref="DeleteGenericAsync"/>
    public abstract Task DeleteAsync(TModel entry, CancellationToken ct = default);
    
    /// <inheritdoc/>
    public async Task<IEnumerable<object>> LoadPageGenericAsync(int page, int perPage, Sorting sorting, CancellationToken ct) {
        return await LoadPageAsync(page, perPage, sorting, ct);
    }
    
    /// <inheritdoc/>
    public async Task<SearchResult> SearchGenericAsync(string searchTerm, int page, int perPage, Sorting sorting, CancellationToken ct) {
        return await SearchAsync(searchTerm, page, perPage, sorting, ct);
    }
    
    /// <inheritdoc/>
    public Task CreateGenericAsync(object entry, CancellationToken ct) {
        return CreateAsync((TModel)entry, ct);
    }

    /// <inheritdoc/>
    public Task UpdateGenericAsync(object entry, CancellationToken ct) {
        return UpdateAsync((TModel)entry, ct);
    }

    /// <inheritdoc/>
    public Task DeleteGenericAsync(object entry, CancellationToken ct) {
        return DeleteAsync((TModel)entry, ct);
    }

    /// <summary>
    /// This method is called, when the repository gets loaded. Override this method if you need to do some custom logic
    /// </summary>
    /// <param name="table">The table for what the repository is used</param>
    public virtual void Initialize(TableConfig table) {}

}