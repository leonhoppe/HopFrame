using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Providers;

public interface IAdminPagesProvider {
    
    internal void RegisterAdminPage(string url, AdminPage page);
    AdminPage LoadAdminPage(string url);
    IList<AdminPage> LoadRegisteredAdminPages();
    AdminPage HasPageFor(Type type);

}