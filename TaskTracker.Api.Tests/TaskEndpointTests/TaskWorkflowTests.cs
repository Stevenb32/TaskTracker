using System.Net;
using System.Net.Http.Json;
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
    public async Task CreateTask_ThenChangePriority_ThenUpdateDetails_ThenComplete_ThenReopen_ReturnsExpectedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store",
            Priority = TaskPriority.Low
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = TaskPriority.Critical
        };

        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{createdTask!.Id}/priority",
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateRequest = new TaskItemUpdateDetailsRequest
        {
            Title = "Buy oat milk",
            Notes = "From Publix"
        };

        var updateResponse = await _client.PutAsJsonAsync($"/tasks/{createdTask.Id}", updateRequest);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var completeResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/complete", null);
        completeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // When
        var reopenResponse = await _client.PostAsync($"/tasks/{createdTask.Id}/reopen", null);

        // Then
        reopenResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getByIdResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await getByIdResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        retrievedTask.Should().NotBeNull();
        retrievedTask!.Id.Should().Be(createdTask.Id);
        retrievedTask.Title.Should().Be(updateRequest.Title);
        retrievedTask.Notes.Should().Be(updateRequest.Notes);
        retrievedTask.Priority.Should().Be(TaskPriority.Critical.ToString());
        retrievedTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
        retrievedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        retrievedTask.CompletedAt.Should().BeNull();
        retrievedTask.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateTask_ThenDeleteTask_ThenGetTask_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store",
            Priority = TaskPriority.Low
        };

        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();

        // When
        var deleteResponse = await _client.DeleteAsync($"/tasks/{createdTask!.Id}");

        // Then
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getByIdResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
