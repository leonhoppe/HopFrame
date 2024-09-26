namespace HopFrame.Database.Models;

public class PermissionGroup : IPermissionOwner {
    public string Name { get; init; }
    public bool IsDefaultGroup { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public IList<Permission> Permissions { get; set; }
}