using System.Collections;

namespace HopFrame.Core.Repositories;

/** The base repository that provides access to the model dataset */
public abstract class HopFrameRepository<TModel> : IHopFrameRepository where TModel : notnull {

    /** <inheritdoc cref="LoadPageGenericAsync"/> */
    public abstract Task<IEnumerable<TModel>> LoadPageAsync(int page, int perPage, CancellationToken ct = default);
    
    /** <inheritdoc/> */
    public abstract Task<int> CountAsync(CancellationToken ct = default);
    
    /** <inheritdoc cref="SearchGenericAsync"/> */
    public abstract Task<IEnumerable<TModel>> SearchAsync(string searchTerm, int page, int perPage, CancellationToken ct = default);

    /** <inheritdoc cref="CreateGenericAsync"/> */
    public abstract Task CreateAsync(TModel entry, CancellationToken ct);
    
    /** <inheritdoc cref="DeleteGenericAsync"/> */
    public abstract Task DeleteAsync(TModel entry, CancellationToken ct);
    
    /** <inheritdoc/> */
    public async Task<IEnumerable> LoadPageGenericAsync(int page, int perPage, CancellationToken ct = default) {
        return await LoadPageAsync(page, perPage, ct);
    }
    
    /** <inheritdoc/> */
    public async Task<IEnumerable> SearchGenericAsync(string searchTerm, int page, int perPage, CancellationToken ct = default) {
        return await SearchAsync(searchTerm, page, perPage, ct);
    }
    
    /** <inheritdoc/> */
    public Task CreateGenericAsync(object entry, CancellationToken ct) {
        return CreateAsync((TModel)entry, ct);
    }
    
    /** <inheritdoc/> */
    public Task DeleteGenericAsync(object entry, CancellationToken ct) {
        return DeleteAsync((TModel)entry, ct);
    }
    
}