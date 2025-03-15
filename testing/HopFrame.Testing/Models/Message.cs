using HopFrame.Core.Repositories;

namespace HopFrame.Testing.Models;

public class Message {
    public required int MessageIdentifier { get; set; }
    public required User Sender { get; set; }
    public required Guest Receiver { get; set; }
    public required string Content { get; set; }
}

public class MessageRepository : IHopFrameRepository<Message, int> {

    public List<Message> Messages { get; } = new();
    
    public async Task<IEnumerable<Message>> LoadPage(int page, int perPage) {
        return Messages
            .Skip(page * perPage)
            .Take(perPage);
    }
    
    public async Task<SearchResult<Message>> Search(string searchTerm, int page, int perPage) {
        var results = Messages
            .Where(message => message.Content.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase))
            .ToArray();

        var totalPages = (int)Math.Ceiling(results.Length / (double)perPage);
        return new SearchResult<Message>(results
            .Skip(page * perPage)
            .Take(perPage), totalPages);
    }
    
    public async Task<int> GetTotalPageCount(int perPage) {
        return (int)Math.Ceiling(Messages.Count / (double)perPage);
    }
    
    public Task CreateItem(Message item) {
        Messages.Add(item);
        return Task.CompletedTask;
    }
    
    public Task EditItem(Message item) {
        var old = Messages.Find(m => m.MessageIdentifier == item.MessageIdentifier);
        if (old is not null)
            Messages.Remove(old);
        
        Messages.Add(item);
        return Task.CompletedTask;
    }
    
    public Task DeleteItem(Message item) {
        Messages.Remove(item);
        return Task.CompletedTask;
    }
    
    public async Task<Message?> GetOne(int key) {
        return Messages.Find(m => m.MessageIdentifier == key);
    }
}
