using HopFrame.Database.Models.Entries;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Database;

public class HopDbContextBase : DbContext {
    
    public HopDbContextBase() {}
    
    public HopDbContextBase(DbContextOptions options) : base(options) {}

    public virtual DbSet<UserEntry> Users { get; set; }
    public virtual DbSet<PermissionEntry> Permissions { get; set; }
    public virtual DbSet<TokenEntry> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntry>();
        modelBuilder.Entity<PermissionEntry>();
    }
}