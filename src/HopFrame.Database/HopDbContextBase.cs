using HopFrame.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database;

/// <summary>
/// This class includes the basic database structure in order for HopFrame to work
/// </summary>
public abstract class HopDbContextBase : DbContext {

    public static IList<Action<HopDbContextBase>> SaveHandlers = new List<Action<HopDbContextBase>>();

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

        modelBuilder.Entity<Token>()
            .HasMany(t => t.Permissions)
            .WithOne(t => t.Token)
            .OnDelete(DeleteBehavior.Cascade);
    }

    private void OnSaving() {
        var orphanedPermissions = Permissions
            .Where(p => p.UserId == null && p.GroupName == null && p.TokenId == null)
            .ToList();

        foreach (var handler in SaveHandlers) {
            handler.Invoke(this);
        }
    
        Permissions.RemoveRange(orphanedPermissions);
    }

    public override int SaveChanges() {
        OnSaving();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess) {
        OnSaving();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken()) {
        OnSaving();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken()) {
        OnSaving();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}