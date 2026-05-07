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
    public async Task ReopenTask_WhenTaskExists_ReturnsOk()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);

        // When        
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reopenedTask = await reopenResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        reopenedTask.Should().NotBeNull();

        reopenedTask.Id.Should().Be(createdTask.Id);
    }

    [Fact]
    public async Task ReopenTask_WhenTaskExists_UpdatesStatusToActive()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);

        // When        
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reopenedTask = await reopenResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        reopenedTask.Should().NotBeNull();

        reopenedTask.Id.Should().Be(createdTask.Id);
        reopenedTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
    }

    [Fact]
    public async Task ReopenTask_WhenTaskExists_ClearsCompletedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);

        // When        
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reopenedTask = await reopenResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        reopenedTask.Should().NotBeNull();

        reopenedTask.Id.Should().Be(createdTask.Id);
        reopenedTask.CompletedAt.Should().BeNull();
    }    

    [Fact]
    public async Task ReopenTask_WhenTaskIsNotCompleted_DoesNotChangeState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        // When        
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var reopenedTask = await reopenResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        reopenedTask.Should().NotBeNull();

        reopenedTask.Id.Should().Be(createdTask.Id);
        reopenedTask.Status.Should().Be(createdTask.Status.ToString());
        reopenedTask.CompletedAt.Should().Be(createdTask.CompletedAt);

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();

        savedTask.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Status.Should().Be(createdTask.Status);
        savedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        savedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
    }
    
} 