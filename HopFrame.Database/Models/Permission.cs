namespace HopFrame.Database.Models;

public sealed class Permission {
    public long Id { get; init; }
    public string PermissionName { get; set; }
    public Guid Owner { get; set; }
    public DateTime GrantedAt { get; set; }
}

public interface IPermissionOwner {}
