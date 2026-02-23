using Microsoft.EntityFrameworkCore;
using TestApplication.Models;

namespace TestApplication;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options) {

    public DbSet<User> Users { get; set; }

    public DbSet<Post> Posts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.Sender)
            .OnDelete(DeleteBehavior.Cascade);
    }
}