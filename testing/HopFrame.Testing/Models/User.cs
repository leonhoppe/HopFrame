namespace HopFrame.Testing.Models;

public class User {
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}