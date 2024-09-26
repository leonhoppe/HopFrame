using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HopFrame.Database.Models;

public class PermissionGroup : IPermissionOwner {
    
    [Key, Required, MaxLength(50)]
    public string Name { get; init; }
    
    [Required, DefaultValue(false)]
    public bool IsDefaultGroup { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public virtual IList<Permission> Permissions { get; set; }
    
}