using Assignment01.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Assignment01.Data;

public class AppDbContext : IdentityDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Purchases)
            .WithOne(p => p.Event)
            .HasForeignKey(p => p.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        var event1Date = new DateTime(2025, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var event2Date = new DateTime(2025, 11, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Event>().HasData(
            new Event
            {
                Id = 1,
                Title = "Event 1",
                Category = "Workshop",
                AvailableTickets = 100,
                EventDate = event1Date,
                PricePerTicket = 50
            },
            new Event
            {
                Id = 2,
                Title = "Event 2",
                Category = "Concert",
                AvailableTickets = 100,
                EventDate = event2Date,
                PricePerTicket = 0
            }
        );
    }
}