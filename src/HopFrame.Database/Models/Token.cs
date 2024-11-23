using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HopFrame.Database.Models;

public class Token {
    public const int RefreshTokenType = 0;
    public const int AccessTokenType = 1;

    /// <summary>
    /// Defines the Type of the stored Token
    /// 0: Refresh token
    /// 1: Access token
    /// </summary>
    [Required, MinLength(1), MaxLength(1)]
    public int Type { get; set; }

    [Key, Required, MinLength(36), MaxLength(36)]
    public Guid Content { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
    
    [ForeignKey("UserId"), JsonIgnore]
    public virtual User Owner { get; set; }
}