using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class ChangePriorityTaskTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ChangePriorityTaskTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ChangePriority_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();

        var changePriorityRequest = new TaskItemChangePriorityRequest
        {
            Priority = TaskPriority.Critical
        };

        // When
        var changePriorityResponse = await _client.PatchAsJsonAsync(
            $"/tasks/{nonExistentId}/priority", 
            changePriorityRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        changePriorityResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }



    // ChangePriority_WhenPriorityIsUnknown_ReturnsBadRequest



    // ChangePriority_WhenPriorityValueIsInvalid_ReturnsBadRequest



    // ChangePriority_WhenPriorityIsNotProvided_ReturnsBadRequest



    // ChangePriority_WhenPriorityIsProvidedAsNumber_ReturnsBadRequest



    // ChangePriority_WhenTaskExists_ReturnsOkWithUpdatedTask



    // ChangePriority_WhenTaskExists_PersistsUpdatedPriority



    // ChangePriority_WhenTaskIsCompleted_UpdatesPriority



    // ChangePriority_WhenPriorityIsUnchanged_ReturnsOkWithoutUpdatingTask




}




























