using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
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
        var response = await _client.DeleteAsync($"/tasks/{nonExistentId}");
    
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);        
    }

    [Fact]
    public async Task DeleteTask_WhenTaskExists_ReturnsNoContent()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);
    
        // When
        var response = await _client.DeleteAsync($"/tasks/{task.Id}");
    
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteTask_WhenTaskExists_RemovesTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);        
    
        // When
        var deleteResponse = await _client.DeleteAsync($"/tasks/{task.Id}");
        var getByIdResponse = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

} 