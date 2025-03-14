namespace HopFrame.Core.Repositories;

public interface IHopFrameRepository<TModel, in TKey> where TModel : class {

    Task<IEnumerable<TModel>> LoadPage(int page, int perPage);

    Task<SearchResult<TModel>> Search(string searchTerm, int page, int perPage);

    Task<int> GetTotalPageCount(int perPage);

    Task CreateItem(TModel item);

    Task EditItem(TModel item);

    Task DeleteItem(TModel item);

    Task<TModel?> GetOne(TKey key);

}

public readonly struct SearchResult<TModel>(IEnumerable<TModel> items, int pageCount) {
    public IEnumerable<TModel> Items { get; init; } = items;
    public int PageCount { get; init; } = pageCount;
}
