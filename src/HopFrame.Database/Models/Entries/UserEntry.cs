using System.ComponentModel.DataAnnotations;

namespace HopFrame.Database.Models.Entries;

public class UserEntry {
    [Key, Required, MinLength(36), MaxLength(36)]
    public string Id { get; set; }
    
    [MaxLength(50)]
    public string Username { get; set; }
    
    [Required, MaxLength(50), EmailAddress]
    public string Email { get; set; }
    
    [Required, MinLength(8), MaxLength(255)]
    public string Password { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}