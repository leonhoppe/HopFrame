using System.ComponentModel.DataAnnotations;

namespace HopFrame.Database.Models.Entries;

public class TokenEntry {
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
    public string Token { get; set; }

    [Required, MinLength(36), MaxLength(36)]
    public string UserId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }
}