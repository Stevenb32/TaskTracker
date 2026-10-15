using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Endpoints;
using TaskTracker.Api.ErrorHandling;
using TaskTracker.Api.Json;

var migrateDatabase = args.Any(
    arg => string.Equals(arg, "--migrate-db", StringComparison.OrdinalIgnoreCase));

var applicationArgs = args
    .Where(arg => !string.Equals(arg, "--migrate-db", StringComparison.OrdinalIgnoreCase))
    .ToArray();

var builder = WebApplication.CreateBuilder(applicationArgs);

// services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    TaskTrackerJsonSerializerOptions.Configure(options.SerializerOptions);
});

var connectionString = builder.Configuration.GetConnectionString("TaskTrackerDb");

builder.Services.AddDbContext<TaskTrackerDbContext>(
    options => options.UseNpgsql(connectionString));

builder.Services.AddValidation();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<DomainValidationExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

if (migrateDatabase)
{
    Console.WriteLine("Applying TaskTracker database migrations...");

    await using var scope = app.Services.CreateAsyncScope();

    var db = scope.ServiceProvider.GetRequiredService<TaskTrackerDbContext>();

    await db.Database.MigrateAsync();

    Console.WriteLine("TaskTracker database migrations completed successfully.");

    return;
}

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("E2E"))
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TaskTracker API v1");
        options.RoutePrefix = "swagger";
    });
}

if (app.Environment.IsEnvironment("E2E"))
{
    app.MapPost("/testing/reset-db", async (TaskTrackerDbContext db) =>
    {
        db.Tasks.RemoveRange(db.Tasks);
        await db.SaveChangesAsync();

        return Results.NoContent();
    });
}

app.UseExceptionHandler();

// endpoints
app.MapGet("/", () => Results.Ok("TaskTracker API is running"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// feature endpoints
app.MapTaskItemEndpoints();

app.Run();

public partial class Program { }