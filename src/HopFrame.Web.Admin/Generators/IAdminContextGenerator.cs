namespace HopFrame.Web.Admin.Generators;

public interface IAdminContextGenerator {

    /// <summary>
    /// Returns the generator object for the specified Admin Page. This needs to be within the same Admin Context.
    /// </summary>
    /// <typeparam name="TModel">The Model of the Admin Page</typeparam>
    /// <returns></returns>
    IAdminPageGenerator<TModel> Page<TModel>();

}