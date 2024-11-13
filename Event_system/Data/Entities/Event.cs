using System.ComponentModel.DataAnnotations;
using Event_system.Auth.Model;

namespace Event_system.Data.Entities;

public class Event
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required decimal Price { get; set; }
    
    public required Category Category { get; set; }

    [Required]
    public required string UserId { get; set; }
    public EventUser User { get; set; }
    public EventDto ToDto()
    {
        return new EventDto(Id,Title, Description, StartDate, EndDate, Price);
    }
    
}