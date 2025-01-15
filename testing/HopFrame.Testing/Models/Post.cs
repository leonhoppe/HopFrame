using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopFrame.Testing.Models;

public class Post {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [MaxLength(255)]
    public required string Caption { get; set; }
    
    public required string Content { get; set; }

    [ForeignKey("author")]
    public User? Author { get; set; }

    public bool Published { get; set; }
}