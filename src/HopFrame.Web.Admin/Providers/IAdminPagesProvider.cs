using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Providers;

public interface IAdminPagesProvider {
    
    AdminPage LoadAdminPage(string url);
    IList<AdminPage> LoadRegisteredAdminPages();
    AdminPage HasPageFor(Type type);

}