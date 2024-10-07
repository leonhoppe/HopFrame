namespace HopFrame.Web.Admin;

public abstract class ModelRepository<TModel> : IModelRepository {
    public abstract Task<IEnumerable<TModel>> ReadAll();
    public abstract Task<TModel> Create(TModel model);
    public abstract Task<TModel> Update(TModel model);
    public abstract Task Delete(TModel model);


    public async Task<IEnumerable<object>> ReadAllO() {
        var models = await ReadAll();
        return models.Select(m => (object)m);
    }

    public async Task<object> CreateO(object model) {
        return await Create((TModel)model);
    }

    public async Task<object> UpdateO(object model) {
        return await Update((TModel)model);
    }

    public Task DeleteO(object model) {
        return Delete((TModel)model);
    }
}

public interface IModelRepository {
    Task<IEnumerable<object>> ReadAllO();
    Task<object> CreateO(object model);
    Task<object> UpdateO(object model);
    Task DeleteO(object model);
}
