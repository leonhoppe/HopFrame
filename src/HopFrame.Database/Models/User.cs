using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HopFrame.Database.Models;

public class User : IPermissionOwner {
    
    [Key, Required, MinLength(36), MaxLength(36)]
    public Guid Id { get; init; }
    
    [Required, MaxLength(50)]
    public string Username { get; set; }
    
    [Required, MaxLength(50), EmailAddress]
    public string Email { get; set; }
    
    [Required, MinLength(8), MaxLength(255), JsonIgnore]
    public string Password { get; set; }
    
    [Required]
    public DateTime CreatedAt { get; set; }
    
    public virtual List<Permission> Permissions { get; set; }
    
    [JsonIgnore]
    public virtual List<Token> Tokens { get; set; }
    
}