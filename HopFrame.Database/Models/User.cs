namespace HopFrame.Database.Models;

public class User {
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<string> Permissions { get; set; }
}