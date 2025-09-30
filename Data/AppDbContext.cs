using Assignment01.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<User> Users { get; set; }
}