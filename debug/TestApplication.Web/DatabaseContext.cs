using Microsoft.EntityFrameworkCore;
using TestApplication.Web.Models;

namespace TestApplication.Web;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options) {

    public DbSet<User> Users { get; set; }

    public DbSet<Post> Posts { get; set; }

    public DbSet<Typer> Typers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasKey(p => p.Id);
        
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.Sender)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Typer>()
            .HasKey(t => t.Id);
    }
}