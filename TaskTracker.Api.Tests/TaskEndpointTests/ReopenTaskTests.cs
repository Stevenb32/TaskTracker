using System.Net;
using System.Net.Http.Json;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class ReopenTaskTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReopenTaskTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReopenTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        // When
        var reopenResponse = await _client.PostAsync($"/tasks/{nonExistentId}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReopenTask_WhenCompletedTaskExists_ReturnsOkWithReopenedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk",
            "From the store",
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        createdTask.Complete(new DateTimeOffset(2026, 3, 25, 8, 0, 0, TimeSpan.Zero));
        await _factory.AddTaskAsync(createdTask);

        var originalUpdatedAt = createdTask.UpdatedAt;
        var timeBeforeReopen = DateTimeOffset.UtcNow;

        // When
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        var timeAfterReopen = DateTimeOffset.UtcNow;

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reopenedTask = await reopenResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        reopenedTask.Should().NotBeNull();
        reopenedTask!.Id.Should().Be(createdTask.Id);
        reopenedTask.Title.Should().Be(createdTask.Title);
        reopenedTask.Notes.Should().Be(createdTask.Notes);
        reopenedTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
        reopenedTask.Priority.Should().Be(createdTask.Priority.ToString());
        reopenedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        reopenedTask.CompletedAt.Should().BeNull();
        reopenedTask.UpdatedAt.Should().NotBe(originalUpdatedAt);
        reopenedTask.UpdatedAt.Should().BeOnOrAfter(timeBeforeReopen);
        reopenedTask.UpdatedAt.Should().BeOnOrBefore(timeAfterReopen);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Status.Should().Be(Domain.TaskStatus.Active);
        savedTask.Priority.Should().Be(createdTask.Priority);
        savedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        savedTask.CompletedAt.Should().BeNull();
        savedTask.UpdatedAt.Should().Be(reopenedTask.UpdatedAt);
    }

    [Fact]
    public async Task ReopenTask_WhenTaskIsNotCompleted_DoesNotChangeState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk",
            "From the store",
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);

        var originalUpdatedAt = createdTask.UpdatedAt;

        // When
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reopenedTask = await reopenResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        reopenedTask.Should().NotBeNull();
        reopenedTask!.Id.Should().Be(createdTask.Id);
        reopenedTask.Title.Should().Be(createdTask.Title);
        reopenedTask.Notes.Should().Be(createdTask.Notes);
        reopenedTask.Status.Should().Be(createdTask.Status.ToString());
        reopenedTask.Priority.Should().Be(createdTask.Priority.ToString());
        reopenedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        reopenedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
        reopenedTask.UpdatedAt.Should().Be(originalUpdatedAt);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Status.Should().Be(createdTask.Status);
        savedTask.Priority.Should().Be(createdTask.Priority);
        savedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        savedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
        savedTask.UpdatedAt.Should().Be(originalUpdatedAt);
    }
}
