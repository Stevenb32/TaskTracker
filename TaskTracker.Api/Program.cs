using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using TaskTracker.Api.Data;
using TaskTracker.Api.Endpoints;


var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaskTrackerDbContext>(options => 
    options.UseInMemoryDatabase("TaskTrackerDb"));

builder.Services.AddProblemDetails();
builder.Services.AddValidation();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TaskTracker API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler();

// Endpoints
app.MapGet("/", () => Results.Ok("TaskTracker API is running"));
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

// feature endpoints
app.MapTaskItemEndpoints();

app.Run();

public partial class Program { }