using System.ComponentModel.DataAnnotations;

namespace TestApplication.Web.Models;

public class Post {
    [Key]
    public Guid Id { get; } = Guid.CreateVersion7();
    
    public required User? Sender { get; set; }
    
    [MaxLength(5000)]
    public required string Message { get; set; }

    public override bool Equals(object? obj) {
        if (obj is Post post) {
            return post.Id == Id;
        }

        return false;
    }

    public override int GetHashCode() => Id.GetHashCode();
}