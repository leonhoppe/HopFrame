using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HopFrame.Database.Models;

public class Permission {
    
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }
    
    [Required, MaxLength(255)]
    public string PermissionName { get; set; }
    
    [Required]
    public DateTime GrantedAt { get; set; }

    [ForeignKey("UserId"), JsonIgnore]
    public virtual User User { get; set; }

    public Guid? UserId { get; set; }

    [ForeignKey("GroupName"), JsonIgnore]
    public virtual PermissionGroup Group { get; set; }

    [MaxLength(255)]
    public string GroupName { get; set; }
    
    [ForeignKey("TokenId"), JsonIgnore]
    public virtual Token Token { get; set; }
    
    public Guid? TokenId { get; set; }
}

public interface IPermissionOwner;
