using HopFrame.Database.Models.Entries;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database;

/// <summary>
/// This class includes the basic database structure in order for HopFrame to work
/// </summary>
public abstract class HopDbContextBase : DbContext {

    public virtual DbSet<UserEntry> Users { get; set; }
    public virtual DbSet<PermissionEntry> Permissions { get; set; }
    public virtual DbSet<TokenEntry> Tokens { get; set; }
    public virtual DbSet<GroupEntry> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntry>();
        modelBuilder.Entity<PermissionEntry>();
        modelBuilder.Entity<TokenEntry>();
        modelBuilder.Entity<GroupEntry>();
    }

    /// <summary>
    /// Gets executed when a user is deleted through the IUserService from the
    /// HopFrame.Security package. You can override this method to also delete
    /// related user specific entries in the database
    /// </summary>
    /// <param name="user"></param>
    public virtual void OnUserDelete(UserEntry user) {}
}