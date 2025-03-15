using HopFrame.Core.Config;
using HopFrame.Core.Repositories;

namespace HopFrame.Core.Services.Implementations;

public class RepositoryTableManager<TModel, TKey>(IHopFrameRepository<TModel, TKey> repo, IContextExplorer explorer, IServiceProvider provider) : ITableManager where TModel : class {
    public async Task<IEnumerable<object>> LoadPage(int page, int perPage = 20) {
        return await repo.LoadPage(page, perPage);
    }
    public async Task<(IEnumerable<object>, int)> Search(string searchTerm, int page = 0, int perPage = 20) {
        var result = await repo.Search(searchTerm, page, perPage);
        return (result.Items, result.PageCount);
    }
    public Task<int> TotalPages(int perPage = 20) {
        return repo.GetTotalPageCount(perPage);
    }
    public Task DeleteItem(object item) {
        return repo.DeleteItem((TModel)item);
    }
    public Task EditItem(object item) {
        return repo.EditItem((TModel)item);
    }
    public Task AddItem(object item) {
        return repo.CreateItem((TModel)item);
    }
    public Task AddAll(IEnumerable<object> items) {
        var tasks = items
            .Select(item => repo.CreateItem((TModel)item))
            .ToList();
        
        return Task.WhenAll(tasks);
    }
    public async Task<object?> GetOne(object key) {
        return await repo.GetOne((TKey)key);
    }
    public async Task<string> DisplayProperty(object? item, PropertyConfig prop, object? value = null, object? enumerableValue = null) {
        var manager = new TableManager<TModel>(null!, null!, explorer, provider);
        return await manager.DisplayProperty(item, prop, value, enumerableValue);
    }
}