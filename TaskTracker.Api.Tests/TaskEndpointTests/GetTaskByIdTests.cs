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
    public async Task GetTaskById_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();        
        
        var nonExistentId = Guid.NewGuid();

        // When        
        var getTaskByIdResponse = await _client.GetAsync($"/tasks/{nonExistentId}"); // search for GUID

        // Then
        getTaskByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);       
    }

    [Fact]
    public async Task GetTaskById_WhenTaskExists_ReturnsOkWithTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));       

        await _factory.AddTaskAsync(createdTask);

        // When        
        var getTaskByIdResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");

        // Then
        getTaskByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await getTaskByIdResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(createdTask.Id);
        retrievedTask.Title.Should().Be(createdTask.Title);
        retrievedTask.Notes.Should().Be(createdTask.Notes);
    }    

    [Fact]
    public async Task GetTaskById_ReturnsCorrectTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var firstCreatedTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));
        var secondCreatedTask = TaskItem.Create("Walk dog", "Around the block", new DateTimeOffset(2026, 3, 25, 8, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(firstCreatedTask);
        await _factory.AddTaskAsync(secondCreatedTask);

        // When        
        var getTaskByIdResponse = await _client.GetAsync($"/tasks/{firstCreatedTask.Id}");

        // Then
        getTaskByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await getTaskByIdResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(firstCreatedTask.Id);
        retrievedTask.Title.Should().Be(firstCreatedTask.Title);
        retrievedTask.Notes.Should().Be(firstCreatedTask.Notes);

        retrievedTask.Id.Should().NotBe(secondCreatedTask.Id);
        retrievedTask.Title.Should().NotBe(secondCreatedTask.Title);
    }
    
} 