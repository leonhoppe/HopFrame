using System.ComponentModel;
using System.Linq.Expressions;

namespace HopFrame.Web.Admin.Generators;

public interface IAdminPageGenerator<TModel> {

    /// <summary>
    /// Sets the title of the Admin Page
    /// </summary>
    /// <param name="title">the specified title</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> Title(string title);
    
    /// <summary>
    /// Sets the description of the Admin Page
    /// </summary>
    /// <param name="description">the specified description</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> Description(string description);

    /// <summary>
    /// Sets the permission needed to view the Admin Page
    /// </summary>
    /// <param name="permission">the specified permission</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> ReadPermission(string permission);
    
    /// <summary>
    /// Sets the permission needed to create a new Entry
    /// </summary>
    /// <param name="permission">the specified permission</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> CreatePermission(string permission);
    
    /// <summary>
    /// Sets the permission needed to update an Entry
    /// </summary>
    /// <param name="permission">the specified permission</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> UpdatePermission(string permission);
    
    /// <summary>
    /// Sets the permission needed to delete an Entry
    /// </summary>
    /// <param name="permission">the specified permission</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> DeletePermission(string permission);

    
    /// <summary>
    /// Enables or disables the create button
    /// </summary>
    /// <param name="show">the specified state</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> ShowCreateButton(bool show);
    
    /// <summary>
    /// Enables or disables the delete button
    /// </summary>
    /// <param name="show">the specified state</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> ShowDeleteButton(bool show);
    
    /// <summary>
    /// Enables or disables the update button
    /// </summary>
    /// <param name="show">the specified state</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> ShowUpdateButton(bool show);

    /// <summary>
    /// Specifies the default sort property and direction
    /// </summary>
    /// <param name="propertyExpression">Which property should be sorted</param>
    /// <param name="direction">In which direction should be sorted</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> DefaultSort<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression, ListSortDirection direction);

    /// <summary>
    /// Specifies the repository provider for the page
    /// </summary>
    /// <typeparam name="TRepository">The specified provider</typeparam>
    /// <returns></returns>
    IAdminPageGenerator<TModel> ConfigureProvider<TRepository>() where TRepository : ModelProvider<TModel>;
    
    
    /// <summary>
    /// Returns the generator of the specified property
    /// </summary>
    /// <param name="propertyExpression">The property</param>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Property<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression);
    
    /// <summary>
    /// Specifies the default property that should be displayed as a property in other listings
    /// </summary>
    /// <param name="propertyExpression">The property</param>
    /// <returns></returns>
    IAdminPageGenerator<TModel> ListingProperty<TProperty>(Expression<Func<TModel, TProperty>> propertyExpression);

}
