using HopFrame.Testing.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Testing;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options) {

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.Author)
            .WithMany(u => u.Posts);
    }
}