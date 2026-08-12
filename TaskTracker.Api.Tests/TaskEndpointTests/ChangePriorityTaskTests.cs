using System.Net;
using System.Net.Http.Json;
using System.Text;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class ChangePriorityTaskTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ChangePriorityTaskTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ChangePriority_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = TaskPriority.Critical
        };

        // When
        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{nonExistentId}/priority",
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangePriority_WhenPriorityIsUnknown_ReturnsBadRequest()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk",
            "From the store",
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = TaskPriority.Unknown
        };

        // When
        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{createdTask.Id}/priority",
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("""{}""")]
    [InlineData("""{"priority":2}""")]
    [InlineData("""{"priority":99}""")]
    [InlineData("""{"priority":"banana"}""")]
    [InlineData("""{"priority":null}""")]
    public async Task ChangePriority_WhenPriorityPayloadIsInvalid_ReturnsBadRequest(string json)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk",
            "From the store",
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // When
        var response = await _client.PatchAsync($"/tasks/{createdTask.Id}/priority", content);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ChangePriority_WhenTaskExists_ReturnsOkAndPersistsUpdatedPriority()
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

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = TaskPriority.Critical
        };

        var timeBeforeChange = DateTimeOffset.UtcNow;

        // When
        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{createdTask.Id}/priority",
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        var timeAfterChange = DateTimeOffset.UtcNow;

        // Then
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var changedTask = await changePriorityResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        changedTask.Should().NotBeNull();
        changedTask!.Id.Should().Be(createdTask.Id);
        changedTask.Title.Should().Be(createdTask.Title);
        changedTask.Notes.Should().Be(createdTask.Notes);
        changedTask.Status.Should().Be(createdTask.Status.ToString());
        changedTask.Priority.Should().Be(changePriorityRequest.Priority.ToString());
        changedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        changedTask.CompletedAt.Should().BeNull();
        changedTask.UpdatedAt.Should().NotBe(originalUpdatedAt);
        changedTask.UpdatedAt.Should().BeOnOrAfter(timeBeforeChange);
        changedTask.UpdatedAt.Should().BeOnOrBefore(timeAfterChange);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Status.Should().Be(createdTask.Status);
        savedTask.Priority.Should().Be(changePriorityRequest.Priority);
        savedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        savedTask.CompletedAt.Should().BeNull();
        savedTask.UpdatedAt.Should().Be(changedTask.UpdatedAt);
    }

    [Fact]
    public async Task ChangePriority_WhenTaskIsCompleted_UpdatesPriority()
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

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = TaskPriority.Critical
        };

        var timeBeforeChange = DateTimeOffset.UtcNow;

        // When
        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{createdTask.Id}/priority",
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        var timeAfterChange = DateTimeOffset.UtcNow;

        // Then
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var changedTask = await changePriorityResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        changedTask.Should().NotBeNull();
        changedTask!.Id.Should().Be(createdTask.Id);
        changedTask.Status.Should().Be(Domain.TaskStatus.Completed.ToString());
        changedTask.Priority.Should().Be(changePriorityRequest.Priority.ToString());
        changedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
        changedTask.UpdatedAt.Should().NotBe(originalUpdatedAt);
        changedTask.UpdatedAt.Should().BeOnOrAfter(timeBeforeChange);
        changedTask.UpdatedAt.Should().BeOnOrBefore(timeAfterChange);
    }

    [Fact]
    public async Task ChangePriority_WhenPriorityIsUnchanged_ReturnsOkWithoutUpdatingTask()
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

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = createdTask.Priority
        };

        // When
        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{createdTask.Id}/priority",
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var changedTask = await changePriorityResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        changedTask.Should().NotBeNull();
        changedTask!.Id.Should().Be(createdTask.Id);
        changedTask.Title.Should().Be(createdTask.Title);
        changedTask.Notes.Should().Be(createdTask.Notes);
        changedTask.Status.Should().Be(createdTask.Status.ToString());
        changedTask.Priority.Should().Be(createdTask.Priority.ToString());
        changedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        changedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
        changedTask.UpdatedAt.Should().Be(originalUpdatedAt);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.UpdatedAt.Should().Be(originalUpdatedAt);
    }
}
