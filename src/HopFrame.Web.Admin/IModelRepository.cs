namespace HopFrame.Web.Admin;

public interface IModelRepository<TModel> {
    Task<IEnumerable<TModel>> ReadAll();
    Task<TModel> Create(TModel model);
    Task<TModel> Update(TModel model);
    Task Delete(TModel model);
}
