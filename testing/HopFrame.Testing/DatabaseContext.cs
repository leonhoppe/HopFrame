using HopFrame.Testing.Models;
using Microsoft.EntityFrameworkCore;

namespace HopFrame.Testing;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options) {

    public DbSet<User> Users { get; set; }
    
}