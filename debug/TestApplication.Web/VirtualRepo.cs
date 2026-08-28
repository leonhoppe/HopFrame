using HopFrame.Core.Repositories;
// ReSharper disable AsyncMethodWithoutAwait

namespace TestApplication.Web;

public class VirtualRepo : HopFrameRepository<Dictionary<string, object?>> {
    private static List<Dictionary<string, object?>> Entries { get; } = new();
    
    public override async Task<IEnumerable<Dictionary<string, object?>>> LoadPageAsync(int page, int perPage, Sorting sorting, CancellationToken ct = default) {
        return Entries
            .Skip(page * perPage)
            .Take(perPage)
            .ToArray();
    }
    
    public override async Task<int> CountAsync(CancellationToken ct = default) {
        return Entries.Count;
    }
    
    public override async Task<SearchResult> SearchAsync(string searchTerm, int page, int perPage, Sorting sorting, CancellationToken ct = default) {
        return new() {
            Result = await LoadPageAsync(page, perPage, sorting, ct),
            PageCount = await CountAsync(ct)
        };
    }
    
    public override Task CreateAsync(Dictionary<string, object?> entry, CancellationToken ct = default) {
        Entries.Add(entry);
        return Task.CompletedTask;
    }
    
    public override Task UpdateAsync(Dictionary<string, object?> entry, CancellationToken ct = default) {
        return Task.CompletedTask;
    }
    
    public override Task DeleteAsync(Dictionary<string, object?> entry, CancellationToken ct = default) {
        Entries.Remove(entry);
        return Task.CompletedTask;
    }

    public override Task<Dictionary<string, object?>?> GetUniqueEntryAsync(object[] keys, CancellationToken ct) {
        return Task.FromResult<Dictionary<string, object?>?>(null);
    }
}