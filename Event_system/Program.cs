var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var topicsGroup = app.MapGroup("/api");

topicsGroup.MapGet("/categories", () => "GET ALL");
topicsGroup.MapGet("/categories/{categoryId}", (int categoryId) => $"GET {categoryId}");
topicsGroup.MapPost("/categories", (CreateCategoryDto dto) => "POST");
topicsGroup.MapPut("/categories/{categoryId}", (UpdateCategoryDto dto,int categoryId) => "PUT");
topicsGroup.MapDelete("/categories/{categoryId}", (int categoryId) => "DELETE");

app.Run();

public record CategoryDto(int Id, string Name);

public record CreateCategoryDto(string Name);

public record UpdateCategoryDto(string Name);

//Todo: Data/Entites/Db context, docker, 