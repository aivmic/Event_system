using System.ComponentModel.DataAnnotations;
using Event_system.Auth.Model;

namespace Event_system.Data.Entities;

public class Rating
{
    public int Id { get; set; }
    public required int Stars { get; set; }
    public required Event Event { get; set; }
    
    public RatingDto ToDto()
    {
        return new RatingDto(Id, Stars);
    }
    [Required]
    public required string UserId { get; set; }
    public EventUser User { get; set; }
}