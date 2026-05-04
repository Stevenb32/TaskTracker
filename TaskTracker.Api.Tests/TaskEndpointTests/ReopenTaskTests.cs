using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
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
    public async Task ReopenTask_WhenTaskExists_ReturnsOk()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task ReopenTask_WhenTaskExists_UpdatesStatusToActive()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask.Id.Should().Be(task.Id);
        responseTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
    }

    [Fact]
    public async Task ReopenTask_WhenTaskExists_ClearsCompletedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask.Id.Should().Be(task.Id);
        responseTask.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task ReopenTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();        

        // When        
        var response = await _client.PostAsync($"/tasks/{nonExistentId}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReopenTask_WhenTaskIsNotCompleted_DoesNotChangeState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask!.Id.Should().Be(task.Id);
        responseTask.Status.Should().Be(task.Status.ToString());
        responseTask.CompletedAt.Should().Be(task.CompletedAt);

        var savedTask = await _factory.GetTaskByIdAsync(task.Id);
        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(task.Id);
        savedTask.Title.Should().Be(task.Title);
        savedTask.Notes.Should().Be(task.Notes);
        savedTask.Status.Should().Be(task.Status);
        savedTask.CreatedAt.Should().Be(task.CreatedAt);
        savedTask.CompletedAt.Should().Be(task.CompletedAt);
    }
} 