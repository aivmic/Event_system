namespace Event_system.Data.Entities;

public class Event
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public int Price { get; set; }
    
    public required Category Category { get; set; }

    public EventDto ToDto()
    {
        return new EventDto(Id,Title, Description, StartDate, EndDate, Price);
    }
    
}