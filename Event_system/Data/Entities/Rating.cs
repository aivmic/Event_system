namespace Event_system.Data.Entities;

public class Rating
{
    public int Id { get; set; }
    public required int stars { get; set; }
    public Event Event { get; set; }
}