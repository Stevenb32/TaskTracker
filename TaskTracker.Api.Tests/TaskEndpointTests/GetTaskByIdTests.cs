using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class GetTaskByIdTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GetTaskByIdTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTaskById_WhenTaskExists_ReturnsOkWithTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var existingTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));       

        await _factory.AddTaskAsync(existingTask);

        // When        
        var response = await _client.GetAsync($"/tasks/{existingTask.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();

        responseTask.Id.Should().Be(existingTask.Id);
        responseTask.Title.Should().Be(existingTask.Title);
        responseTask.Notes.Should().Be(existingTask.Notes);
    }

    [Fact]
    public async Task GetTaskById_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();        
        
        var nonExistentId = Guid.NewGuid();

        // When        
        var response = await _client.GetAsync($"/tasks/{nonExistentId}"); // search for GUID

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);       
    }

    [Fact]
    public async Task GetTaskById_ReturnsCorrectTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task1 = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));
        var task2 = TaskItem.Create("Walk dog", "Around the block", new DateTimeOffset(2026, 3, 25, 8, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task1);
        await _factory.AddTaskAsync(task2);

        // When        
        var response = await _client.GetAsync($"/tasks/{task1.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();

        responseTask.Id.Should().Be(task1.Id);
        responseTask.Title.Should().Be(task1.Title);
        responseTask.Notes.Should().Be(task1.Notes);

        responseTask.Id.Should().NotBe(task2.Id);
        responseTask.Title.Should().NotBe(task2.Title);
    }
} 