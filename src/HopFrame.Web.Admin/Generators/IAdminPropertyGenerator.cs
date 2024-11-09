using System.Linq.Expressions;

namespace HopFrame.Web.Admin.Generators;

public interface IAdminPropertyGenerator<TProperty, TModel> {

    IAdminPropertyGenerator<TProperty, TModel> Sortable(bool sortable);
    IAdminPropertyGenerator<TProperty, TModel> Editable(bool editable);
    IAdminPropertyGenerator<TProperty, TModel> DisplayValueWhileEditing(bool display);
    IAdminPropertyGenerator<TProperty, TModel> DisplayInListing(bool display = true);
    IAdminPropertyGenerator<TProperty, TModel> Ignore(bool ignore = true);
    IAdminPropertyGenerator<TProperty, TModel> Generated(bool generated = true);
    IAdminPropertyGenerator<TProperty, TModel> Bold(bool bold = true);
    IAdminPropertyGenerator<TProperty, TModel> Unique(bool unique = true);
    
    IAdminPropertyGenerator<TProperty, TModel> DisplayName(string displayName);
    IAdminPropertyGenerator<TProperty, TModel> Description(string description);
    IAdminPropertyGenerator<TProperty, TModel> Prefix(string prefix);
    IAdminPropertyGenerator<TProperty, TModel> Validator(Func<TProperty, string> validator);
    IAdminPropertyGenerator<TProperty, TModel> IsSelector(bool selector = true);
    IAdminPropertyGenerator<TProperty, TModel> IsSelector<TSelectorType>(bool selector = true);
    IAdminPropertyGenerator<TProperty, TModel> Parser(Func<TModel, string, TProperty> parser);
    IAdminPropertyGenerator<TProperty, TModel> Parser<TInput>(Func<TModel, TInput, TProperty> parser);
    IAdminPropertyGenerator<TProperty, TModel> Parser<TInput, TInnerProperty>(Func<TModel, TInput, TInnerProperty> parser);
    IAdminPropertyGenerator<TProperty, TModel> DisplayProperty(Expression<Func<TProperty, object>> propertyExpression);
    IAdminPropertyGenerator<TProperty, TModel> DisplayProperty<TInnerProperty>(Expression<Func<TInnerProperty, object>> propertyExpression);

}