using System.ComponentModel;
using System.Text.Json.Serialization;

namespace HopFrame.Web.Admin.Models;

public class AdminPage<TModel> : IAdminPageEntry {
    public string Title { get; set; }
    public string Description { get; set; }
    public AdminPagePermissions Permissions { get; set; }
    public IList<AdminPageProperty> Properties { get; set; }
    [JsonIgnore]
    public Type RepositoryProvider { get; set; }
    
    public string DefaultSortPropertyName { get; set; }
    public ListSortDirection DefaultSortDirection { get; set; }

    public bool ShowCreateButton { get; set; } = true;
    public bool ShowDeleteButton { get; set; } = true;
    public bool ShowUpdateButton { get; set; } = true;
}

public interface IAdminPageEntry;
