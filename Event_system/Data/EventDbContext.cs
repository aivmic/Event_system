using Event_system.Auth.Model;
using Event_system.Data.Entities;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Event_system.Data;

public class EventDbContext : IdentityDbContext<EventUser>
{
    private readonly IConfiguration _configuration;
    public DbSet<Category> Categories { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public EventDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("PostgreSQL"));
    }
}