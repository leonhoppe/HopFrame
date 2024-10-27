using System.Linq.Expressions;

namespace HopFrame.Web.Admin.Generators;

public interface IAdminPropertyGenerator<TProperty> {

    IAdminPropertyGenerator<TProperty> Sortable(bool sortable);
    IAdminPropertyGenerator<TProperty> Editable(bool editable);
    IAdminPropertyGenerator<TProperty> DisplayValueWhileEditing(bool display);
    IAdminPropertyGenerator<TProperty> DisplayInListing(bool display = true);
    IAdminPropertyGenerator<TProperty> Ignore(bool ignore = true);
    IAdminPropertyGenerator<TProperty> Generated(bool generated = true);
    IAdminPropertyGenerator<TProperty> Bold(bool bold = true);
    IAdminPropertyGenerator<TProperty> Unique(bool unique = true);
    
    IAdminPropertyGenerator<TProperty> DisplayName(string displayName);
    IAdminPropertyGenerator<TProperty> Description(string description);
    IAdminPropertyGenerator<TProperty> Prefix(string prefix);
    IAdminPropertyGenerator<TProperty> Validator(Func<TProperty, string> validator);
    IAdminPropertyGenerator<TProperty> IsSelector<TSelector>();
    IAdminPropertyGenerator<TProperty> Parser<TModel>(Func<TModel, string, TProperty> parser);
    IAdminPropertyGenerator<TProperty> ParserForListType<TModel, TInnerProperty>(Func<TModel, string, TInnerProperty> parser);
    IAdminPropertyGenerator<TProperty> DisplayProperty<TListingProperty>(Expression<Func<TProperty, TListingProperty>> propertyExpression);
    IAdminPropertyGenerator<TProperty> DisplayPropertyForListType<TInnerProperty>(Expression<Func<TInnerProperty, object>> propertyExpression);

}