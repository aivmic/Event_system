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
            var category = new Category{Name = dto.Name, Description = dto.Description};
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

    public static void AddEventApi(this WebApplication app)
    {
        var eventsGroup = app.MapGroup("/api/categories/{categoryId}").AddFluentValidationAutoValidation();
        
        eventsGroup.MapGet("/events", async(EventDbContext dbContext) =>
        {
            return (await dbContext.Events.ToListAsync()).Select(x => x.ToDto());
        });
        
        eventsGroup.MapGet("/events/{eventId}", async (int categoryId, int eventId, EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            return @event == null ? Results.NotFound() : TypedResults.Ok(@event.ToDto());
        });
        
        //
        eventsGroup.MapPost("/events", async (int categoryId, CreateEventDto dto, EventDbContext dbContext) =>
        {
            var @event = new Event{Title = dto.Title, Description = dto.Description, StartDate = dto.StartDate, EndDate = dto.EndDate, Price = dto.Price, categoryId = categoryId};
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
        
    }
}