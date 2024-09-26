namespace HopFrame.Database.Models;

public sealed class User : IPermissionOwner {
    public Guid Id { get; init; }
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<Permission> Permissions { get; set; }
}