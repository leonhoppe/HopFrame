using System.ComponentModel;
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
    public virtual required User Author { get; set; }

    public bool Published { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateOnly Created { get; set; }

    public TimeOnly At { get; set; }

    public ListSortDirection Type { get; set; }
}