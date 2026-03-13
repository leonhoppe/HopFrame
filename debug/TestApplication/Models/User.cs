using System.ComponentModel.DataAnnotations;

namespace TestApplication.Models;

public class User {
    [Key]
    public Guid Id { get; } = Guid.CreateVersion7();

    public int Index { get; set; }
    
    [EmailAddress, MaxLength(25)]
    public required string Email { get; set; }
    
    [MaxLength(64)]
    public required string Password { get; set; }
    
    [MaxLength(25)]
    public required string FirstName { get; set; }
    
    [MaxLength(25)]
    public required string LastName { get; set; }

    [MaxLength(255)]
    public string? Description { get; set; }

    public required DateOnly Birth { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public List<Post> Posts { get; set; } = new();

    public override bool Equals(object? obj) {
        if (obj is User other) {
            return other.Id == Id;
        }

        return false;
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }

    public override string ToString() {
        return Email;
    }
}