using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database;

/// <summary>
/// This class includes the basic database structure in order for HopFrame to work
/// </summary>
public abstract class HopDbContextBase : DbContext {

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Permission> Permissions { get; set; }
    public virtual DbSet<Token> Tokens { get; set; }
    public virtual DbSet<PermissionGroup> Groups { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Tokens)
            .WithOne(t => t.Owner)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Permissions)
            .WithOne(p => p.User)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PermissionGroup>()
            .HasMany(g => g.Permissions)
            .WithOne(p => p.Group)
            .OnDelete(DeleteBehavior.Cascade);
    }
}