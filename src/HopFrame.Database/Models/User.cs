using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Members;

namespace HopFrame.Database.Models;

[AdminDescription("On this page you can manage all user accounts.")]
public class User : IPermissionOwner {
    
    [Key, Required, MinLength(36), MaxLength(36)]
    public Guid Id { get; init; }
    
    [MaxLength(50)]
    public string Username { get; set; }
    
    [Required, MaxLength(50), EmailAddress]
    public string Email { get; set; }
    
    [Required, MinLength(8), MaxLength(255), JsonIgnore, AdminIgnore(onlyForListing: true), AdminHideValue]
    public string Password { get; set; }
    
    [Required, AdminUneditable]
    public DateTime CreatedAt { get; set; }
    
    [AdminIgnore(onlyForListing: true)]
    public virtual IList<Permission> Permissions { get; set; }
    
    [JsonIgnore, AdminIgnore]
    public virtual IList<Token> Tokens { get; set; }
    
}