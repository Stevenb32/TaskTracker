using System.Net;
using System.Net.Http.Json;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class CompleteTaskTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CompleteTaskTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CompleteTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        // When
        var completeResponse = await _client.PostAsync($"/tasks/{nonExistentId}/complete", null);

        // Then
        completeResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CompleteTask_WhenTaskExists_ReturnsOkWithCompletedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk",
            "From the store",
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);

        var timeBeforeComplete = DateTimeOffset.UtcNow;

        // When
        var completeResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);

        var timeAfterComplete = DateTimeOffset.UtcNow;

        // Then
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var completedTask = await completeResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        completedTask.Should().NotBeNull();
        completedTask!.Id.Should().Be(createdTask.Id);
        completedTask.Title.Should().Be(createdTask.Title);
        completedTask.Notes.Should().Be(createdTask.Notes);
        completedTask.Status.Should().Be(Domain.TaskStatus.Completed.ToString());
        completedTask.Priority.Should().Be(createdTask.Priority.ToString());
        completedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        completedTask.CompletedAt.Should().NotBeNull();
        completedTask.CompletedAt.Should().BeOnOrAfter(timeBeforeComplete);
        completedTask.CompletedAt.Should().BeOnOrBefore(timeAfterComplete);
        completedTask.UpdatedAt.Should().Be(completedTask.CompletedAt);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(completedTask.Id);
        savedTask.Title.Should().Be(completedTask.Title);
        savedTask.Notes.Should().Be(completedTask.Notes);
        savedTask.Status.Should().Be(Domain.TaskStatus.Completed);
        savedTask.Priority.Should().Be(createdTask.Priority);
        savedTask.CreatedAt.Should().Be(completedTask.CreatedAt);
        savedTask.CompletedAt.Should().Be(completedTask.CompletedAt);
        savedTask.UpdatedAt.Should().Be(completedTask.UpdatedAt);
    }

    [Fact]
    public async Task CompleteTask_WhenTaskAlreadyCompleted_DoesNotChangeState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk",
            "From the store",
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);

        var firstCompleteResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);
        firstCompleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstCompletedTask = await firstCompleteResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        firstCompletedTask.Should().NotBeNull();

        // When
        var secondCompleteResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);

        // Then
        secondCompleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondCompletedTask = await secondCompleteResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        secondCompletedTask.Should().NotBeNull();
        secondCompletedTask!.Id.Should().Be(firstCompletedTask!.Id);
        secondCompletedTask.Title.Should().Be(firstCompletedTask.Title);
        secondCompletedTask.Notes.Should().Be(firstCompletedTask.Notes);
        secondCompletedTask.Status.Should().Be(firstCompletedTask.Status);
        secondCompletedTask.Priority.Should().Be(firstCompletedTask.Priority);
        secondCompletedTask.CreatedAt.Should().Be(firstCompletedTask.CreatedAt);
        secondCompletedTask.CompletedAt.Should().Be(firstCompletedTask.CompletedAt);
        secondCompletedTask.UpdatedAt.Should().Be(firstCompletedTask.UpdatedAt);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(firstCompletedTask.Id);
        savedTask.Title.Should().Be(firstCompletedTask.Title);
        savedTask.Notes.Should().Be(firstCompletedTask.Notes);
        savedTask.Status.Should().Be(Domain.TaskStatus.Completed);
        savedTask.Priority.Should().Be(createdTask.Priority);
        savedTask.CreatedAt.Should().Be(firstCompletedTask.CreatedAt);
        savedTask.CompletedAt.Should().Be(firstCompletedTask.CompletedAt);
        savedTask.UpdatedAt.Should().Be(firstCompletedTask.UpdatedAt);
    }
}
