using HopFrame.Web.Admin.Models;

namespace HopFrame.Web.Admin.Providers.Implementation;

public class AdminPagesProvider(IServiceProvider provider) : IAdminPagesProvider {
    private static readonly IDictionary<string, PageDataStore> Pages = new Dictionary<string, PageDataStore>();

    public static void RegisterAdminPage<TContext>(string url, Type pageType) where TContext : AdminPagesContext {
        Pages.Add(url, new PageDataStore {
            ContextType = typeof(TContext),
            PageType = pageType
        });
    }

    public AdminPage LoadAdminPage(string url) {
        if (!Pages.TryGetValue(url, out var data)) return null;

        var context = provider.GetService(data.ContextType);
        var property = data.ContextType.GetProperties()
            .SingleOrDefault(prop => prop.PropertyType == data.PageType);

        return property?.GetValue(context) as AdminPage;
    }

    public IList<AdminPage> LoadRegisteredAdminPages() {
        return Pages
            .Select(pair => LoadAdminPage(pair.Key))
            .ToList();
    }

    public AdminPage HasPageFor(Type type) {
        foreach (var (url, data) in Pages) {
            var innerType = data.PageType.GenericTypeArguments[0];
            if (innerType != type) continue;
            return LoadAdminPage(url);
        }

        return null;
    }
}

internal struct PageDataStore {
    public Type PageType { get; set; }
    public Type ContextType { get; set; }
}
