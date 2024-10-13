using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Providers.Implementation;

public class AdminPagesProvider : IAdminPagesProvider {
    private readonly IDictionary<string, AdminPage> _pages = new Dictionary<string, AdminPage>();
    
    public void RegisterAdminPage(string url, AdminPage page) {
        _pages.Add(url, page);
    }

    public AdminPage LoadAdminPage(string url) {
        return _pages.TryGetValue(url, out var page) ? page : null;
    }

    public IList<AdminPage> LoadRegisteredAdminPages() {
        return _pages.Values.ToList();
    }

    public AdminPage HasPageFor(Type type) {
        return _pages
            .Where(p => p.Value.ModelType == type)
            .Select(p => p.Value)
            .SingleOrDefault();
    }
}