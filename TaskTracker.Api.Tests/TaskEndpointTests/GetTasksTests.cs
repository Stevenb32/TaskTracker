using System.Net;
using System.Net.Http.Json;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class GetTasksTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GetTasksTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_WhenNoTasksExist_ReturnsOkWithEmptyList()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        // When        
        var getTasksResponse = await _client.GetAsync("/tasks");

        // Then
        getTasksResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await getTasksResponse.Content.ReadFromJsonAsync<List<TaskItemResponse>>();

        tasks.Should().NotBeNull();

        tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTasks_WhenTasksExist_ReturnsOkWithTasks()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var firstCreatedTask = TaskItem.Create(
            "Buy milk", 
            "From the store", 
            TaskPriority.High, 
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        var secondCreatedTask = TaskItem.Create(
            "Walk dog", 
            "Around the block", 
            TaskPriority.Critical, 
            new DateTimeOffset(2026, 3, 25, 8, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(firstCreatedTask);
        await _factory.AddTaskAsync(secondCreatedTask);

        // When        
        var getTasksResponse = await _client.GetAsync("/tasks");

        // Then
        getTasksResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await getTasksResponse.Content.ReadFromJsonAsync<List<TaskItemResponse>>();

        tasks.Should().NotBeNull();

        tasks.Should().HaveCount(2);
        tasks.Should().Contain(
            t => t.Id == firstCreatedTask.Id && 
            t.Title == firstCreatedTask.Title && 
            t.Notes == firstCreatedTask.Notes && 
            t.Status == firstCreatedTask.Status.ToString() && 
            t.Priority == firstCreatedTask.Priority.ToString());
        tasks.Should().Contain(
            t => t.Id == secondCreatedTask.Id && 
            t.Title == secondCreatedTask.Title && 
            t.Notes == secondCreatedTask.Notes && 
            t.Status == secondCreatedTask.Status.ToString() && 
            t.Priority == secondCreatedTask.Priority.ToString());
    }

    [Fact]
    public async Task GetTasks_WhenMultipleTasksExist_ReturnsTasksOrderedByCreatedAtDescending()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var olderTask = TaskItem.Create(
            "Older task", 
            "Older notes", TaskPriority.High, 
            new DateTimeOffset(2025, 5, 19, 10, 0, 0, TimeSpan.Zero));

        var newerTask = TaskItem.Create(
            "Newer task", 
            "Newer notes", 
            TaskPriority.Critical, 
            new DateTimeOffset(2026, 5, 20, 10, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(olderTask);
        await _factory.AddTaskAsync(newerTask);

        // When
        var getTasksResponse = await _client.GetAsync("/tasks");

        // Then
        getTasksResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await getTasksResponse.Content.ReadFromJsonAsync<List<TaskItemResponse>>();

        tasks.Should().NotBeNull();
        tasks.Select(task => task.Id).Should().ContainInOrder(newerTask.Id, olderTask.Id);
    }

}