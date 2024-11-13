using System.Security.Claims;
using Event_system.Auth.Model;
using Event_system.Data;
using Event_system.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
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

        categoryGroup.MapPost("/categories", [Authorize(Roles = EventRoles.EventUser)] async (CreateCategoryDto dto, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var category = new Category { Name = dto.Name, Description = dto.Description, UserId = userId };
            dbContext.Categories.Add(category);

            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"api/categories/{category.Id}", category.ToDto());
        });

        categoryGroup.MapPut("/categories/{categoryId}", [Authorize] async (UpdateCategoryDto dto, int categoryId, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!httpContext.User.IsInRole(EventRoles.Admin) && userId != category.UserId)
            {
                return Results.Forbid();
            }

            category.Description = dto.Description;

            dbContext.Categories.Update(category);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(category.ToDto());
        });

        categoryGroup.MapDelete("/categories/{categoryId}", [Authorize] async (int categoryId, EventDbContext dbContext, HttpContext httpContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!httpContext.User.IsInRole(EventRoles.Admin) && userId != category.UserId)
            {
                return Results.Forbid();
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }

    public static void AddEventApi(this WebApplication app)
    {
        var eventsGroup = app.MapGroup("/api/categories/{categoryId}").AddFluentValidationAutoValidation();

        eventsGroup.MapGet("/events", async (int categoryId, EventDbContext dbContext) =>
        {
            var events = await dbContext.Events
                .Where(e => e.Category.Id == categoryId)
                .ToListAsync();

            return events.Select(x => x.ToDto());
        });

        eventsGroup.MapGet("/events/{eventId}", async (int categoryId, int eventId, EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            return @event == null ? Results.NotFound() : TypedResults.Ok(@event.ToDto());
        });

        eventsGroup.MapPost("/events", [Authorize(Roles = EventRoles.EventUser)] async (int categoryId, CreateEventDto dto, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            if (category == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var @event = new Event
            {
                Title = dto.Title,
                Description = dto.Description,
                StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc),
                Price = dto.Price,
                Category = category,
                UserId = userId
            };

            dbContext.Events.Add(@event);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"api/categories/{categoryId}/events/{@event.Id}", @event.ToDto());
        });

        eventsGroup.MapPut("/events/{eventId}", [Authorize] async (UpdateEventDto dto, int categoryId, int eventId, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            if (@event == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!httpContext.User.IsInRole(EventRoles.Admin) && userId != @event.UserId)
            {
                return Results.Forbid();
            }

            @event.Description = dto.Description;

            dbContext.Events.Update(@event);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(@event.ToDto());
        });

        eventsGroup.MapDelete("/events/{eventId}", [Authorize] async (int categoryId, int eventId, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var @event = await dbContext.Events.FindAsync(eventId);
            if (@event == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!httpContext.User.IsInRole(EventRoles.Admin) && userId != @event.UserId)
            {
                return Results.Forbid();
            }

            dbContext.Events.Remove(@event);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }

    public static void AddRatingApi(this WebApplication app)
    {
        var ratingsGroup = app.MapGroup("/api/categories/{categoryId}/events/{eventId}").AddFluentValidationAutoValidation();

        ratingsGroup.MapGet("/ratings", async (int eventId, EventDbContext dbContext) =>
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

        ratingsGroup.MapPost("/ratings", [Authorize(Roles = EventRoles.EventUser)] async (int categoryId, int eventId, CreateRatingDto dto, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var category = await dbContext.Categories.FindAsync(categoryId);
            var @event = await dbContext.Events.FindAsync(eventId);
            if (category == null || @event == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var rating = new Rating { Stars = dto.Stars, Event = @event, UserId = userId };

            dbContext.Ratings.Add(rating);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"api/categories/{categoryId}/events/{eventId}/ratings/{rating.Id}", rating.ToDto());
        });

        ratingsGroup.MapPut("/ratings/{ratingId}", [Authorize] async (UpdateRatingDto dto, int categoryId, int eventId, int ratingId, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var rating = await dbContext.Ratings.FindAsync(ratingId);
            if (rating == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!httpContext.User.IsInRole(EventRoles.Admin) && userId != rating.UserId)
            {
                return Results.Forbid();
            }

            rating.Stars = dto.Stars;

            dbContext.Ratings.Update(rating);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(rating.ToDto());
        });

        ratingsGroup.MapDelete("/ratings/{ratingId}", [Authorize] async (int categoryId, int ratingId, HttpContext httpContext, EventDbContext dbContext) =>
        {
            var rating = await dbContext.Ratings.FindAsync(ratingId);
            if (rating == null)
            {
                return Results.NotFound();
            }

            var userId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (!httpContext.User.IsInRole(EventRoles.Admin) && userId != rating.UserId)
            {
                return Results.Forbid();
            }

            dbContext.Ratings.Remove(rating);
            await dbContext.SaveChangesAsync();

            return TypedResults.NoContent();
        });
    }
}
