using System.Collections;

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
namespace HopFrame.Core.Repositories;

/** The generic repository that provides access to the model dataset */
public interface IHopFrameRepository {

    /// <summary>
    /// Loads a whole page of entries
    /// </summary>
    /// <param name="page">The index of the current page (starts at 0)</param>
    /// <param name="perPage">The amount of entries that should be loaded</param>
    public Task<IEnumerable> LoadPageGenericAsync(int page, int perPage, CancellationToken ct = default);
    
    /// <summary>
    /// Returns the total amount of entries in the dataset
    /// </summary>
    public Task<int> CountAsync(CancellationToken ct = default);
    
    /// <summary>
    /// Searches through the whole dataset and returns a page of matching entries
    /// </summary>
    /// <param name="searchTerm">The search text provided by the user</param>
    /// <param name="page">The index of the current page (starts at 0)</param>
    /// <param name="perPage">The amount of entries that should be loaded</param>
    public Task<IEnumerable> SearchGenericAsync(string searchTerm, int page, int perPage, CancellationToken ct = default);

    
    /// <summary>
    /// Saves the newly created entry to the dataset
    /// </summary>
    /// <param name="entry">The entry that needs to be saved</param>
    public Task CreateGenericAsync(object entry, CancellationToken ct);
    
    /// <summary>
    /// Deletes the provided entry from the dataset
    /// </summary>
    /// <param name="entry">The entry that needs to be deleted</param>
    public Task DeleteGenericAsync(object entry, CancellationToken ct);

}