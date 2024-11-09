using System.Linq.Expressions;

namespace HopFrame.Web.Admin.Generators;

public interface IAdminPropertyGenerator<TProperty, TModel> {

    /// <summary>
    /// Should the property be sortable or not
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Sortable(bool sortable);
    
    /// <summary>
    /// Should the admin be able to edit the property after creation or not
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Editable(bool editable);
    
    /// <summary>
    /// Should the value of the property be displayed while editing or not (useful for passwords and tokens)
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> DisplayValueWhileEditing(bool display);
    
    /// <summary>
    /// Should the property be a column on the page list or not
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> DisplayInListing(bool display = true);
    
    /// <summary>
    /// Should the property be ignored completely
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Ignore(bool ignore = true);
    
    /// <summary>
    /// Is the value of the property database generated and is not meant to be changed
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Generated(bool generated = true);
    
    /// <summary>
    /// Should the property value be bold in the listing or not
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Bold(bool bold = true);
    
    /// <summary>
    /// Is the value of the property unique under all other entries in the dataset
    /// </summary>
    /// <param name="unique"></param>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Unique(bool unique = true);
    
    /// <summary>
    /// Specifies the display name in the listing and editing/creation
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> DisplayName(string displayName);
    
    /// <summary>
    /// Has the value of the property a never changing prefix that doesn't need to be specified or displayed
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Prefix(string prefix);
    
    /// <summary>
    /// The specified function gets called before creation/edit to verify that the entered value matches the property requirements
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Validator(Func<TProperty, string> validator);
    
    /// <summary>
    /// Sets the input type in creation/edit to a selector for the property type. The property type needs to have its own admin page in order for the selector to work!
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> IsSelector(bool selector = true);
    
    /// <summary>
    /// Sets the input type in creation/edit to a selector for the specified type. The specified type needs to have its own admin page in order for the selector to work!
    /// </summary>
    /// <param name="selector"></param>
    /// <typeparam name="TSelectorType"></typeparam>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> IsSelector<TSelectorType>(bool selector = true);
    
    /// <summary>
    /// The specified function gets called, whenever the entry is changed/created in order to convert the raw string input to the proper property type
    /// </summary>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Parser(Func<TModel, string, TProperty> parser);
    
    /// <summary>
    /// The specified function gets called, whenever the entry is changed/created in order to convert the raw string input to the proper property type
    /// </summary>
    /// <typeparam name="TInput">Needs to be specified if the field is not a plain string field (like a selector with a different type)</typeparam>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Parser<TInput>(Func<TModel, TInput, TProperty> parser);
    
    /// <summary>
    /// The specified function gets called, whenever the entry is changed/created in order to convert the raw string input to the proper property type
    /// </summary>
    /// <typeparam name="TInput">Needs to be specified if the field is not a plain string field (like a selector with a different type)</typeparam>
    /// <typeparam name="TInnerProperty">Needs to be specified if the property type is a List</typeparam>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> Parser<TInput, TInnerProperty>(Func<TModel, TInput, TInnerProperty> parser);
    
    /// <summary>
    /// Specifies the default property that should be displayed as a value
    /// </summary>
    /// <param name="propertyExpression"></param>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> DisplayProperty(Expression<Func<TProperty, object>> propertyExpression);
    
    /// <summary>
    /// Specifies the default property that should be displayed as a value
    /// </summary>
    /// <typeparam name="TInnerProperty">Needs to be specified if the property type is a List</typeparam>
    /// <returns></returns>
    IAdminPropertyGenerator<TProperty, TModel> DisplayProperty<TInnerProperty>(Expression<Func<TInnerProperty, object>> propertyExpression);

}