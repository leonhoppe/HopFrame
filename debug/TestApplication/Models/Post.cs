using System.ComponentModel.DataAnnotations;

namespace TestApplication.Models;

public class Post {
    [Key]
    public Guid Id { get; } = Guid.CreateVersion7();
    
    public required User? Sender { get; set; }
    
    [MaxLength(5000)]
    public required string Message { get; set; }
}