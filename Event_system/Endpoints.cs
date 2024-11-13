using Event_system.Data;
using Event_system.Data.Entities;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Event_system;

public static class Endpoints
{
    public static void AddCategoryApi(this WebApplication app)
    {
        var categoryGroup = app.MapGroup("/api").AddFluentValidationAutoValidation();

        categoryGroup.MapGet("/categories", async(EventDbContext dbContext) =>
        {
            return (await dbContext.Categories.ToListAsync()).Select(x => x.ToDto());
        });
        categoryGroup.MapGet("/categories/{categoryId}", async (int categoryId, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            return category == null ? Results.NotFound() : TypedResults.Ok(category.ToDto());
        });
        categoryGroup.MapPost("/categories", async (CreateCategoryDto dto, EventDbContext dbContext) =>
        {
            var category = new Category{Name = dto.Name, Description = dto.Description, UserId = ""};
            dbContext.Categories.Add(category);
    
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Created($"api/categories/{category.Id}", category.ToDto());
        });
        categoryGroup.MapPut("/categories/{categoryId}", async (UpdateCategoryDto dto,int categoryId, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return Results.NotFound();
            }
            category.Description = dto.Description;
    
    
            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Ok(category.ToDto());

        });
        categoryGroup.MapDelete("/categories/{categoryId}", async(int categoryId,EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return Results.NotFound();
            }
    
            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }

    public static void AddRatingApi(this WebApplication app)
    {
        var ratingsGroup = app.MapGroup("/api/categories/{categoryId}/events/{eventId}").AddFluentValidationAutoValidation();
        
        ratingsGroup.MapGet("/ratings", async(int eventId, EventDbContext dbContext) =>
        {
            var ratings = await dbContext.Ratings
                .Where(r => r.Event.Id == eventId)
                .ToListAsync();
            
            return ratings.Select(x => x.ToDto());
        });
        
        ratingsGroup.MapGet("/ratings/{ratingId}", async (int ratingId, EventDbContext dbContext) =>
        {
            var rating = await dbContext.Ratings.FindAsync(ratingId);
            return rating == null ? Results.NotFound() : TypedResults.Ok(rating.ToDto());
        });
        
        ratingsGroup.MapPost("/ratings", async (int categoryId,int eventId,CreateRatingDto dto, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            var rating = new Rating { Stars = dto.Stars, Event = await dbContext.Events.FindAsync(eventId), UserId = ""};
            dbContext.Ratings.Add(rating);
            if(category == null || rating.Event == null)
            {
                return Results.NotFound();
            }
            await dbContext.SaveChangesAsync();
            
            return TypedResults.Created($"api/categories/{categoryId}/events/{eventId}/ratings/{rating.Id}", rating.ToDto());
        });
        ratingsGroup.MapPut("/ratings/{ratingId}", async (UpdateRatingDto dto,int categoryId,int eventId,int ratingId, EventDbContext dbContext) =>
        {
            var rating = await dbContext.Ratings.FindAsync(ratingId);
            if (rating == null)
            {
                return Results.NotFound();
            }
            rating.Stars = dto.Stars;
            
            dbContext.Ratings.Update(rating);
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Ok(rating.ToDto());
        });
        ratingsGroup.MapDelete("/ratings/{ratingId}", async(int categoryId,int ratingId,EventDbContext dbContext) =>
        {
            var rating = await dbContext.Ratings.FindAsync(ratingId);
            if (rating == null)
            {
                return Results.NotFound();
            }
    
            dbContext.Ratings.Remove(rating);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }
    public static void AddEventApi(this WebApplication app)
    {
        var eventsGroup = app.MapGroup("/api/categories/{categoryId}").AddFluentValidationAutoValidation();
        
        eventsGroup.MapGet("/events", async(int categoryId, EventDbContext dbContext) =>
        {
            var ratings = await dbContext.Events
                .Where(r => r.Category.Id == categoryId)
                .ToListAsync();
            
            return ratings.Select(x => x.ToDto());
        });
        
        eventsGroup.MapGet("/events/{eventId}", async (int categoryId, int eventId, EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            return @event == null ? Results.NotFound() : TypedResults.Ok(@event.ToDto());
        });
        
        //
        eventsGroup.MapPost("/events", async (int categoryId, CreateEventDto dto, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            var @event = new Event{Title = dto.Title, Description = dto.Description, StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc), Price = dto.Price, Category = await dbContext.Categories.FindAsync(categoryId), UserId = ""};
            dbContext.Events.Add(@event);
    
            if(category == null)
            {
                return Results.NotFound();
            }
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Created($"api/categories/{categoryId}/events/{@event.Id}", @event.ToDto());
        });
        eventsGroup.MapPut("/events/{eventId}", async (UpdateEventDto dto,int categoryId,int eventId, EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            if (@event == null)
            {
                return Results.NotFound();
            }
            @event.Description = dto.Description;
            
            dbContext.Events.Update(@event);
            await dbContext.SaveChangesAsync();
    
            return TypedResults.Ok(@event.ToDto());
        });
        eventsGroup.MapDelete("/events/{eventId}", async(int categoryId,int eventId,EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            if (@event == null)
            {
                return Results.NotFound();
            }
    
            dbContext.Events.Remove(@event);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }
}