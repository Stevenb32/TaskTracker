using Microsoft.EntityFrameworkCore;
using TaskTracker.Api.Data;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Endpoints;

public static class TaskItemEndpoints
{
    public static IEndpointRouteBuilder MapTaskItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/tasks").WithTags("Tasks");

        group.MapPost("/", CreateTask)
            .WithName("CreateTask")
            .Produces<TaskItemResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/", GetTasks)
            .WithName("GetTasks")
            .Produces<List<TaskItemResponse>>(StatusCodes.Status200OK);

        return app;
    }

    private static async Task<IResult> CreateTask(TaskItemCreateRequest request, TaskTrackerDbContext db)
    {
        var taskItem = TaskItem.Create(
            request.Title,
            request.Notes,
            DateTimeOffset.UtcNow);

        db.Tasks.Add(taskItem);
        await db.SaveChangesAsync();

        var response = ToResponse(taskItem);

        return Results.Created($"/tasks/{taskItem.Id}", response);
    }

    private static async Task<IResult> GetTasks(TaskTrackerDbContext db)
    {
        var taskItems = await db.Tasks
            .AsNoTracking()
            .ToListAsync();

        var response = taskItems
            .Select(ToResponse)
            .ToList();

        return Results.Ok(response);
    }

    private static TaskItemResponse ToResponse(TaskItem taskItem)
    {
        return new TaskItemResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Notes = taskItem.Notes,
            Status = taskItem.Status.ToString(),
            CreatedAt = taskItem.CreatedAt,
            CompletedAt = taskItem.CompletedAt
        };
    }
}