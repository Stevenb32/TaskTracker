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
    public async Task UpdateTaskDetails_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid(); 

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = "Updated title",
            Notes = "Updated notes"
        };       

        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{nonExistentId}", updateRequest);

        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);        
    }

    [Fact]
    public async Task UpdateTaskDetails_WhenTitleIsMissing_ReturnsBadRequest()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updateRequest = new
        {
            Notes = "Updated notes"
        };
    
        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);
    
        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    public async Task UpdateTaskDetails_WhenTitleIsNullEmptyOrWhitespace_ReturnsBadRequest(string? title)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = title!,
            Notes = "Updated notes"
        };       

        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);

        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public async Task UpdateTaskDetails_WhenTitleExceeds100Characters_ReturnsBadRequest(int titleLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = new string('a', titleLength),
            Notes = "Updated notes"
        };       

        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);

        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public async Task UpdateTaskDetails_WhenNotesExceeds500Characters_ReturnsBadRequest(int notesLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = "Updated title",
            Notes = new string('a', notesLength)
        };       

        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);

        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]    
    public async Task UpdateTaskDetails_WhenTitleIs1To100Characters_ReturnsOk(int titleLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = new string('a', titleLength),
            Notes = "Updated notes"
        };       

        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);

        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTaskDetails_WhenTaskExists_ReturnsOkWithUpdatedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = "Updated title",
            Notes = "Updated notes"
        };
    
        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);
    
        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        updatedTask.Should().NotBeNull();
        
        updatedTask.Id.Should().Be(createdTask.Id);
        updatedTask.Title.Should().Be(updateRequest.Title);
        updatedTask.Notes.Should().Be(updateRequest.Notes);
        updatedTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
        updatedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        updatedTask.CompletedAt.Should().BeNull();
        updatedTask.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTaskDetails_WhenNotesIsNull_ClearsNotes()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var updatgeRequest = new TaskItemUpdateDetailsRequest
        {
            Title = "Updated title",
            Notes = null
        };
    
        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updatgeRequest);
    
        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        updatedTask.Should().NotBeNull();

        updatedTask.Id.Should().Be(createdTask.Id);
        updatedTask.Title.Should().Be(updatgeRequest.Title);
        updatedTask.Notes.Should().BeNull();
        updatedTask.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateTaskDetails_WhenTitleAndNotesAreUnchanged_ReturnsOkWithoutChangingUpdatedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createdTask = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(createdTask);

        var originalUpdatedAt = createdTask.UpdatedAt;

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = createdTask.Title,
            Notes = createdTask.Notes
        };

        // When
        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);
    
        // Then
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedTask = await updateResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        updatedTask.Should().NotBeNull();  

        updatedTask.Id.Should().Be(createdTask.Id);
        updatedTask.Title.Should().Be(createdTask.Title);
        updatedTask.Notes.Should().Be(createdTask.Notes);
        updatedTask.UpdatedAt.Should().Be(originalUpdatedAt);
    }

}