using System.Reflection;
using HopFrame.Core.Config;

namespace HopFrame.Core.Services;

public interface ITableManager {
    public IQueryable<object> LoadPage(int page, int perPage = 20);
    public (IEnumerable<object>, int) Search(string searchTerm, int page = 0, int perPage = 20);
    public int TotalPages(int perPage = 20);
    public Task DeleteItem(object item);
    
    public string DisplayProperty(object item, PropertyInfo info, TableConfig? tableConfig);
}