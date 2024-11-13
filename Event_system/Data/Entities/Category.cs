using System.ComponentModel.DataAnnotations;
using Event_system.Auth.Model;

namespace Event_system.Data.Entities;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }

    public CategoryDto ToDto()
    {
        return new CategoryDto(Id, Name, Description);
    }
    [Required]
    public required string UserId { get; set; }
    public EventUser User { get; set; }
}