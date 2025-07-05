using System.ComponentModel.DataAnnotations;

namespace HopFrame.Testing.Models;

public class User {
    [Key]
    public required Guid Id { get; init; } = Guid.CreateVersion7();
    public required string Email { get; init; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    public virtual List<Post> Posts { get; set; } = new();
}