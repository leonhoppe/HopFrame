using System.ComponentModel;
using System.Linq.Expressions;

namespace HopFrame.Web.Admin.Generators;

public interface IAdminPageGenerator<TModel> {

    IAdminPageGenerator<TModel> Title(string title);
    IAdminPageGenerator<TModel> Description(string description);
    IAdminPageGenerator<TModel> Url(string url);

    IAdminPageGenerator<TModel> ViewPermission(string permission);
    IAdminPageGenerator<TModel> CreatePermission(string permission);
    IAdminPageGenerator<TModel> UpdatePermission(string permission);
    IAdminPageGenerator<TModel> DeletePermission(string permission);

    IAdminPageGenerator<TModel> ShowCreateButton(bool show);
    IAdminPageGenerator<TModel> ShowDeleteButton(bool show);
    IAdminPageGenerator<TModel> ShowUpdateButton(bool show);

    IAdminPageGenerator<TModel> DefaultSort<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, ListSortDirection direction);

    IAdminPageGenerator<TModel> ConfigureRepository<TRepository>() where TRepository : ModelRepository<TModel>;
    
    IAdminPropertyGenerator Property<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression);
    IAdminPageGenerator<TModel> ListingProperty<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression);

}
