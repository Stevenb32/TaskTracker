using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class TaskWorkflowTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TaskWorkflowTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_ThenGetTask_ReturnsCreatedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();

        var getByIdResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");
        
        // Then
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await getByIdResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(createdTask.Id);
        retrievedTask.Title.Should().Be(createdTask.Title);
        retrievedTask.Notes.Should().Be(createdTask.Notes);
        retrievedTask.Status.Should().Be(createdTask.Status);
        retrievedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        retrievedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
    }

    [Fact]
    public async Task CompleteTask_ThenGetTaskById_ReturnsUpdatedStatus()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When
        var response = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(task.Id);
        retrievedTask.Status.Should().Be(Domain.TaskStatus.Completed.ToString());
        retrievedTask.CompletedAt.Should().NotBeNull(); 
    }
 
    [Fact]
    public async Task ReopenTask_ThenGetTaskById_ReturnsUpdatedStatus()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);
        await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // When
        
        var response = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(task.Id);
        retrievedTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());        
    }

    [Fact]
    public async Task ReopenTask_ThenGetTaskById_ClearsCompletedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);
        await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // When
        
        var response = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(task.Id);
        retrievedTask.CompletedAt.Should().BeNull();        
    }
} 