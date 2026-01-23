namespace HopFrame.Core.Repositories;

/// <summary>
/// The middleware used to access data for custom repositories
/// </summary>
/// <typeparam name="TModel">The type of the entity that is managed by this repository</typeparam>
/// <typeparam name="TKey">The type of the primary key of the entity that is managed by this repository</typeparam>
public interface IHopFrameRepository<TModel, in TKey> where TModel : class {

    /// <summary>
    /// Gets used to load all entities of the current page to display
    /// </summary>
    /// <param name="page">The page the user is currently on (starts at 0)</param>
    /// <param name="perPage">The maximum number of elements that should be returned</param>
    /// <returns>The entries for the requested page</returns>
    Task<IEnumerable<TModel>> LoadPage(int page, int perPage);

    /// <summary>
    /// Gets used to filter the entries by the entered search term
    /// </summary>
    /// <param name="searchTerm">The search text that was entered by the user</param>
    /// <param name="page">The page the user is currently on (starts at 0)</param>
    /// <param name="perPage">The maximum number of elements that should be returned</param>
    /// <returns>A list of the filtered entries on the current page with the total number of pages</returns>
    /// <seealso cref="SearchResult{TModel}"/>
    Task<SearchResult<TModel>> Search(string searchTerm, int page, int perPage);

    /// <summary>
    /// Gets used to determine the total number of pages
    /// </summary>
    /// <param name="perPage">The maximum number of elements per page</param>
    /// <returns>The total number of pages</returns>
    Task<int> GetTotalPageCount(int perPage);

    /// <summary>
    /// Gets used when an entry is created through the UI
    /// </summary>
    /// <param name="item">The entry that needs to be saved</param>
    Task CreateItem(TModel item);

    /// <summary>
    /// Gets used when an entry is modified through the UI
    /// </summary>
    /// <param name="item">The modified entry that needs to be saved</param>
    Task EditItem(TModel item);

    /// <summary>
    /// Gets used when an entry is deleted through the UI
    /// </summary>
    /// <param name="item">The entry that should be deleted</param>
    Task DeleteItem(TModel item);

    /// <summary>
    /// Gets used to receive information about a specific entry from the dataset
    /// </summary>
    /// <param name="key">The primary key of the desired entry</param>
    /// <returns>Either the entry if one is found or <c>null</c></returns>
    Task<TModel?> GetOne(TKey key);

}

/// <summary>
/// The object that is passed to the UI to display the search results
/// </summary>
/// <param name="items">The found entries on the current page</param>
/// <param name="pageCount">The total number of pages that the search contains</param>
/// <typeparam name="TModel">The type of the entity</typeparam>
public readonly struct SearchResult<TModel>(IEnumerable<TModel> items, int pageCount) {
    /// <summary>
    /// The found entries on the current page
    /// </summary>
    public IEnumerable<TModel> Items { get; init; } = items;
    
    /// <summary>
    /// The total number of pages that the search contains
    /// </summary>
    public int PageCount { get; init; } = pageCount;
}
