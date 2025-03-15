using HopFrame.Core.Repositories;

namespace HopFrame.Testing.Models;

public class Guest {
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Message> Messages { get; set; } = new();
}

public class GuestRepository : IHopFrameRepository<Guest, int> {

    public List<Guest> Guests { get; } = new();
    
    public async Task<IEnumerable<Guest>> LoadPage(int page, int perPage) {
        return Guests
            .Skip(page * perPage)
            .Take(perPage);
    }
    
    public async Task<SearchResult<Guest>> Search(string searchTerm, int page, int perPage) {
        var results = Guests
            .Where(message => message.Name.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        var totalPages = (int)Math.Ceiling(results.Length / (double)perPage);
        return new SearchResult<Guest>(results
            .Skip(page * perPage)
            .Take(perPage), totalPages);
    }
    
    public async Task<int> GetTotalPageCount(int perPage) {
        return (int)Math.Ceiling(Guests.Count / (double)perPage);
    }
    
    public Task CreateItem(Guest item) {
        Guests.Add(item);
        return Task.CompletedTask;
    }
    
    public Task EditItem(Guest item) {
        var old = Guests.Find(m => m.Id == item.Id);
        if (old is not null)
            Guests.Remove(old);
        
        Guests.Add(item);
        return Task.CompletedTask;
    }
    
    public Task DeleteItem(Guest item) {
        Guests.Remove(item);
        return Task.CompletedTask;
    }
    
    public async Task<Guest?> GetOne(int key) {
        return Guests.Find(m => m.Id == key);
    }
}
