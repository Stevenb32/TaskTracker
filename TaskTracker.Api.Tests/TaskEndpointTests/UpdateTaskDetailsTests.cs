using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class UpdateTaskDetailsTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UpdateTaskDetailsTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UpdateTaskDetails_WhenTaskExists_ReturnsOkWithUpdatedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        var request = new TaskItemUpdateDetailsRequest
        {
            Title = "Updated title",
            Notes = "Updated notes"
        };
    
        // When
        var response = await _client.PutAsJsonAsync($"/tasks/{task.Id}", request);
    
        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();

        updatedTask.Should().NotBeNull();
        updatedTask!.Id.Should().Be(task.Id);
        updatedTask.Title.Should().Be(request.Title);
        updatedTask.Notes.Should().Be(request.Notes);
        updatedTask.Status.Should().Be("Active");
        updatedTask.CreatedAt.Should().Be(task.CreatedAt);
        updatedTask.CompletedAt.Should().BeNull();
        updatedTask.UpdatedAt.Should().NotBeNull();
    }














    // UpdateTaskDetails_WhenTaskDoesNotExist_ReturnsNotFound
    // UpdateTaskDetails_WhenTitleIsMissing_ReturnsBadRequest
    // UpdateTaskDetails_WhenNotesIsNull_ClearsNotes
    // UpdateTaskDetails_WhenTitleAndNotesAreUnchanged_ReturnsOkWithoutChangingUpdatedAt


}