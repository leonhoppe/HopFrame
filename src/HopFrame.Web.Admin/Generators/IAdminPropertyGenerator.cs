namespace HopFrame.Web.Admin.Generators;

public interface IAdminPropertyGenerator {

    IAdminPropertyGenerator Sortable(bool sortable);
    IAdminPropertyGenerator Editable(bool editable);
    IAdminPropertyGenerator DisplayValueWhileEditing(bool display);
    IAdminPropertyGenerator DisplayInListing(bool display = true);
    IAdminPropertyGenerator Ignore(bool ignore = true);
    IAdminPropertyGenerator Generated(bool generated = true);
    IAdminPropertyGenerator Bold(bool bold = true);
    
    IAdminPropertyGenerator DisplayName(string displayName);
    IAdminPropertyGenerator Description(string description);
    IAdminPropertyGenerator Prefix(string prefix);
    IAdminPropertyGenerator Validator(Func<object, bool> validator);
    IAdminPropertyGenerator IsSelector<TSelector>();

}