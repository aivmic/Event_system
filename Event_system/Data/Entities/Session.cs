using System.ComponentModel.DataAnnotations;
using Event_system.Auth.Model;

namespace Event_system.Data.Entities;

public class Session
{
    public Guid Id { get; set; }
    public string LastRefreshToken { get; set; }
    public DateTimeOffset InitiatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(3).ToUniversalTime();
    public bool IsRevoked { get; set; }
    [Required]
    public required string UserId { get; set; }
    public EventUser User { get; set; }
}