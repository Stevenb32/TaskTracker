using System.Net;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class DeleteTaskTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DeleteTaskTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DeleteTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        // When
        var deleteResponse = await _client.DeleteAsync($"/tasks/{nonExistentId}");
    
        // Then
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);        
    }

    [Fact]
    public async Task DeleteTask_WhenTaskExists_ReturnsNoContent()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk", 
            "From the store", 
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);
    
        // When
        var deleteResponse = await _client.DeleteAsync($"/tasks/{createdTask.Id}");
    
        // Then
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_WhenTaskExists_RemovesTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create(
            "Buy milk", 
            "From the store", 
            TaskPriority.Low,
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(createdTask);        
    
        // When
        var deleteResponse = await _client.DeleteAsync($"/tasks/{createdTask.Id}");
        var getByIdResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");

        // Then
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

} 