using System.Reflection.Metadata.Ecma335;
using Event_system;
using Event_system.Data;
using Event_system.Data.Entities;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Results;
using SharpGrip.FluentValidation.AutoValidation.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EventDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation(configuration =>
{
    configuration.OverrideDefaultResultFactoryWith<ProblemDetailsResultFactory>();
});

var app = builder.Build();

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

public record EventDto(int Id, string Title, string Description, DateTime StartDate, DateTime EndDate, int Price);

public record CreateEventDto(string Title, string Description, DateTime StartDate, DateTime EndDate, int Price)
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
public record UpdateEventDto(string Title, string Description, DateTime StartDate, DateTime EndDate, int Price, int CategoryId)
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

public record CreateRatingDto(int Stars);

public record UpdateRatingDto(int Stars);
