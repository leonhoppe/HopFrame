using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestApplication.Web.Models;

public class Post {
    [Key]
    public Guid Id { get; } = Guid.CreateVersion7();

    public Guid SenderId { get; set; }

    [ForeignKey(nameof(SenderId))]
    public required User? Sender { get; set; }
    
    [MaxLength(5000)]
    public required string Message { get; set; }

    [MaxLength(255)]
    public string? Type { get; set; } = "Normal";

    public override bool Equals(object? obj) {
        if (obj is Post post) {
            return post.Id == Id;
        }

        return false;
    }

    public override int GetHashCode() => Id.GetHashCode();
}