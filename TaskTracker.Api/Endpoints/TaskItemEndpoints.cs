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

        group.MapGet("/{id:guid}", GetTaskById)
            .WithName("GetTaskById")
            .Produces<TaskItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id:guid}", UpdateTaskDetails)
            .WithName("UpdateTaskDetails")
            .Produces<TaskItemResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/complete", CompleteTask)
            .WithName("CompleteTask")
            .Produces<TaskItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/{id:guid}/reopen", ReopenTask)
            .WithName("ReopenTask")
            .Produces<TaskItemResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteTask)
            .WithName("DeleteTask")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        

        return app;
    }

    
    // ===============================================================================================================================    
    #region Endpoint Handlers
    // ===============================================================================================================================    
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

    private static async Task<IResult> GetTaskById(Guid id, TaskTrackerDbContext db)
    {
        var taskItem = await db.Tasks
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (taskItem is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(ToResponse(taskItem));
    }

    private static async Task<IResult> UpdateTaskDetails(Guid id, TaskItemUpdateDetailsRequest request, TaskTrackerDbContext db)
    {
        var taskItem = await db.Tasks
            .FirstOrDefaultAsync(t => t.Id == id);

        if (taskItem is null)
        {
            return Results.NotFound();
        }

        taskItem.UpdateDetails(
            request.Title,
            request.Notes,
            DateTimeOffset.UtcNow);

        await db.SaveChangesAsync();

        return Results.Ok(ToResponse(taskItem));
    }

    private static async Task<IResult> CompleteTask(Guid id, TaskTrackerDbContext db)
    {
        var taskItem = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (taskItem is null)
        {
            return Results.NotFound();
        }

        taskItem.Complete(DateTimeOffset.UtcNow);
        await db.SaveChangesAsync();

        return Results.Ok(ToResponse(taskItem));
    }

    private static async Task<IResult> ReopenTask(Guid id, TaskTrackerDbContext db)
    {
        var taskItem = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (taskItem is null)
        {
            return Results.NotFound();
        }

        taskItem.Reopen(DateTimeOffset.UtcNow);
        await db.SaveChangesAsync();

        return Results.Ok(ToResponse(taskItem));
    }

    private static async Task<IResult> DeleteTask(Guid id, TaskTrackerDbContext db)
    {
        var taskItem = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

        if (taskItem is null)
        {
            return Results.NotFound();
        }

        db.Tasks.Remove(taskItem);        
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
    #endregion

    // mapping Helper
    private static TaskItemResponse ToResponse(TaskItem taskItem)
    {
        return new TaskItemResponse
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            Notes = taskItem.Notes,
            Status = taskItem.Status.ToString(),
            CreatedAt = taskItem.CreatedAt,
            CompletedAt = taskItem.CompletedAt,
            UpdatedAt = taskItem.UpdatedAt
        };
    }
}