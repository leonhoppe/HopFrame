using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using HopFrame.Web.Admin.Attributes;
using HopFrame.Web.Admin.Attributes.Members;

namespace HopFrame.Database.Models;

[AdminName("Groups")]
[AdminDescription("On this page you can view, create, edit and delete permission groups.")]
public class PermissionGroup : IPermissionOwner {
    
    [Key, Required, MaxLength(50)]
    public string Name { get; init; }
    
    [Required, DefaultValue(false), AdminUnsortable]
    public bool IsDefaultGroup { get; set; }
    
    [MaxLength(500)]
    public string Description { get; set; }
    
    [Required, AdminUneditable]
    public DateTime CreatedAt { get; set; }
    
    [AdminIgnore(onlyForListing: true)]
    public virtual IList<Permission> Permissions { get; set; }
    
}