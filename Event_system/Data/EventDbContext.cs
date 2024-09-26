using Event_system.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Event_system.Data;

public class EventDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    public EventDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}