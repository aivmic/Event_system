using Event_system;
using Event_system.Data;
using FluentValidation;
using FluentValidation.Results;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<EventDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});

builder.Services.AddEndpointsApiExplorer();

// Add Swagger/OpenAPI services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    // Resolve conflicts temporarily
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

var app = builder.Build();

// Register the endpoints from Endpoints.cs
// Endpoints.AddCategoryApi(app);
// Endpoints.AddEventApi(app);
// Endpoints.AddRatingApi(app);

// Enable middleware to serve generated Swagger as a JSON endpoint.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = false; // Serialize as OpenAPI 3.0
    });
    
    // Serve Swagger UI at the root URL /swagger
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
}

//OpenApi
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.AddCategoryApi();
app.AddEventApi();
app.AddRatingApi();

app.Run();

public class ProblemDetailsResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IResult CreateResult(EndpointFilterInvocationContext context, ValidationResult validationResult)
    {
        var problemDetails = new HttpValidationProblemDetails(validationResult.ToValidationProblemErrors())
        { 
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "Unprocessable Entity",
            Status = 422,
        };
        return TypedResults.Problem(problemDetails);
    }
}

public record CategoryDto(int Id, string Name, string Description);
 

public record CreateCategoryDto(string Name, string Description)
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().Length(min: 3, max: 50);
            RuleFor(x => x.Description).NotEmpty().Length(min: 5, max: 200);
        }
    }
};

public record UpdateCategoryDto(string Description)
{
    public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryDtoValidator()
        {
            RuleFor(x => x.Description).NotEmpty().Length(min: 5, max: 200);
        }
    }
};

public record EventDto(int Id, string Title, string Description, DateTime StartDate, DateTime EndDate, decimal Price);

public record CreateEventDto(string Title, string Description, DateTime StartDate, DateTime EndDate, decimal Price)
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().Length(min: 3, max: 50);
            RuleFor(x => x.Description).NotEmpty().Length(min: 5, max: 200);
            RuleFor(x => x.StartDate).NotEmpty();
            RuleFor(x => x.EndDate).NotEmpty();
            RuleFor(x => x.Price).NotEmpty();
        }
    }
};
public record UpdateEventDto(string Title, string Description, DateTime StartDate, DateTime EndDate, decimal Price, int CategoryId)
{
    public class UpdateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public UpdateEventDtoValidator()
        {
            RuleFor(x => x.Description).NotEmpty().Length(min: 5, max: 200);
        }
    }
}

public record RatingDto(int Id, int Stars);

public record CreateRatingDto(int Stars)
{
    public class CreateRatingDtoValidator : AbstractValidator<CreateRatingDto>
    {
        public CreateRatingDtoValidator()
        {
            RuleFor(x => x.Stars).InclusiveBetween(1, 5);
        }
    }
}

public record UpdateRatingDto(int Stars)
{
    public class UpdateRatingDtoValidator : AbstractValidator<CreateRatingDto>
    {
        public UpdateRatingDtoValidator()
        {
            RuleFor(x => x.Stars).InclusiveBetween(1, 5);
        }
    }
}  
