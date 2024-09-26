using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HopFrame.Database.Models.Entries;

public class GroupEntry {
    [Key, Required, MaxLength(50)]
    public string Name { get; set; }
    
    [Required, DefaultValue(false)]
    public bool Default { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
}