namespace HopFrame.Web.Admin.Generators;

public interface IAdminContextGenerator {

    IAdminPageGenerator<TModel> Page<TModel>();

}