using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HopFrame.Database.Models.Entries;

public sealed class PermissionEntry {
    [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long RecordId { get; set; }

    [Required, MaxLength(255)]
    public string PermissionText { get; set; }

    [Required, MinLength(36), MaxLength(36)]
    public string UserId { get; set; }

    [Required]
    public DateTime GrantedAt { get; set; }
}