namespace HopFrame.Core.Services;

public interface ITableManager {
    public Task<IEnumerable<object>> LoadPage(int page, int perPage = 25);
}