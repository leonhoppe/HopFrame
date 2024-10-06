namespace HopFrame.Web.Admin.Generators;

public interface IAdminPropertyGenerator {

    IAdminPropertyGenerator Sortable(bool sortable);
    IAdminPropertyGenerator Editable(bool editable);
    IAdminPropertyGenerator DisplayValueWhileEditing(bool display);
    IAdminPropertyGenerator DisplayInListing(bool display = true);
    IAdminPropertyGenerator Ignore(bool ignore = true);
    IAdminPropertyGenerator Generated(bool generated = true);
    
    IAdminPropertyGenerator DisplayName(string displayName);
    IAdminPropertyGenerator Description(string description);
    IAdminPropertyGenerator Prefix(string prefix);

}