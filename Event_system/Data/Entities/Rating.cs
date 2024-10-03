namespace Event_system.Data.Entities;

public class Rating
{
    public int Id { get; set; }
    public required int Stars { get; set; }
    public Event Event { get; set; }
    
    public RatingDto ToDto()
    {
        return new RatingDto(Id, Stars);
    }
}