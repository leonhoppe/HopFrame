using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HopFrame.Database.Models;

public class Token : IPermissionOwner {
    public const int RefreshTokenType = 0;
    public const int AccessTokenType = 1;
    public const int ApiTokenType = 2;
    public const int OpenIdTokenType = 3;

    /// <summary>
    /// Defines the Type of the stored Token
    /// 0: Refresh token
    /// 1: Access token
    /// 2: Api token
    /// </summary>
    [Required, MinLength(1), MaxLength(1)]
    public int Type { get; set; }

    [Key, Required, MinLength(36), MaxLength(36)]
    public Guid TokenId { get; set; }

    /// <summary>
    /// Defines the creation date of the token
    /// In case of an api token it defines the date it becomes invalid
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [ForeignKey("UserId"), JsonIgnore]
    public virtual User Owner { get; set; }
    
    public virtual List<Permission> Permissions { get; set; }
}