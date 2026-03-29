using System.ComponentModel;

#pragma warning disable CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)
namespace HopFrame.Core.Repositories;

/// The generic repository that provides access to the model dataset
public interface IHopFrameRepository {

    /// <summary>
    /// Loads a whole page of entries
    /// </summary>
    /// <param name="page">The index of the current page (starts at 0)</param>
    /// <param name="perPage">The amount of entries that should be loaded</param>
    public Task<IEnumerable<object>> LoadPageGenericAsync(int page, int perPage, Sorting sorting, CancellationToken ct);
    
    /// <summary>
    /// Returns the total amount of entries in the dataset
    /// </summary>
    public Task<int> CountAsync(CancellationToken ct);
    
    /// <summary>
    /// Searches through the whole dataset and returns a page of matching entries
    /// </summary>
    /// <param name="searchTerm">The search text provided by the user</param>
    /// <param name="page">The index of the current page (starts at 0)</param>
    /// <param name="perPage">The amount of entries that should be loaded</param>
    public Task<SearchResult> SearchGenericAsync(string searchTerm, int page, int perPage, Sorting sorting, CancellationToken ct);

    
    /// <summary>
    /// Saves the newly created entry to the dataset
    /// </summary>
    /// <param name="entry">The entry that needs to be saved</param>
    public Task CreateGenericAsync(object entry, CancellationToken ct);

    /// <summary>
    /// Saves the changes made to the entry to the dataset
    /// </summary>
    /// <param name="entry">The modified entry</param>
    public Task UpdateGenericAsync(object entry, CancellationToken ct);
    
    /// <summary>
    /// Deletes the provided entry from the dataset
    /// </summary>
    /// <param name="entry">The entry that needs to be deleted</param>
    public Task DeleteGenericAsync(object entry, CancellationToken ct);

}

/// <summary>
/// The result that is returned when a search is requested
/// </summary>
/// <param name="Result">The resulting paginated dataset</param>
/// <param name="PageCount">The total number of pages of the search results</param>
public readonly record struct SearchResult(IEnumerable<object> Result, int PageCount);

/// <summary>
/// Indicates if the returned dataset should be sorted
/// </summary>
/// <param name="PropertyIdentifier">The unique identifier of the property that should be sorted (if null, sorting can be ignored)</param>
/// <param name="Direction">The direction of the sorting</param>
public readonly record struct Sorting(string? PropertyIdentifier, ListSortDirection Direction);
