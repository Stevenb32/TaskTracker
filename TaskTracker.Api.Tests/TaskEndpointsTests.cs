using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;


namespace TaskTracker.Api.Tests;

public class TaskEndpointsTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    // MethodName_WhenCondition_ExpectedResult   
    
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TaskEndpointsTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }


    // create tests

    // Core tests
    
    [Fact]
    public async Task CreateTask_WithValidRequest_ReturnsCreated()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();
        createdTask!.Title.Should().Be("Buy milk");
        createdTask.Notes.Should().Be("From the store");
    }




    // CreateTask_WithValidRequest_PersistsTask
    // CreateTask_ResponseContainsCreatedTask


    // Validation tests
    // CreateTask_WithEmptyTitle_ReturnsBadRequest
    // CreateTask_WithInvalidPriority_ReturnsBadRequest
    // CreateTask_WithMissingRequiredFields_ReturnsBadRequest


    // Edge behavior
    // CreateTask_SetsCreatedDate
    // CreateTask_SetsInitialStatusToTodo





















    // get tests

    // [Fact]
    // public async Task GetTasks_WhenNoTasksExist_ReturnsOkWithEmptyList()
    // {
    //     // Given
    //     await _factory.ResetDatabaseAsync();

    //     // When
    //     var response = await _client.GetAsync("/tasks");

    //     // Then
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);

    //     var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();
    //     tasks.Should().NotBeNull();
    //     tasks.Should().BeEmpty();
    // }

    // [Fact]
    // public async Task GetTasks_WhenTasksExist_ReturnsOkWithTasks()
    // {
    //     // Given
    //     await _factory.ResetDatabaseAsync();

    //     var task = TaskItem.Create(
    //         "Buy milk",
    //         "From the store",
    //         new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

    //     await _factory.AddTaskAsync(task);

    //     // When
    //     var response = await _client.GetAsync("/tasks");

    //     // Then
    //     response.StatusCode.Should().Be(HttpStatusCode.OK);

    //     var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();
    //     tasks.Should().NotBeNull();
    //     tasks.Should().HaveCount(1);
    //     tasks![0].Title.Should().Be("Buy milk");
    // }



    // GetTaskById_WhenTaskExists_ReturnsOkWithTask
    // GetTaskById_WhenTaskDoesNotExist_ReturnsNotFound
    // GetTaskById_ReturnsCorrectTask
    // GetTaskById_ResponseContainsExpectedFields











    // [Fact]
    // public async Task GetTasks_ReturnsOk()
    // {
    //     var response = await _client.GetAsync("/tasks");

    //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    // }


    // complete tests


    // CompleteTask_WhenTaskExists_ReturnsOk
    // CompleteTask_WhenTaskExists_UpdatesStatusToDone
    // CompleteTask_WhenTaskExists_SetsCompletedDate
    // CompleteTask_WhenTaskDoesNotExist_ReturnsNotFound
    // CompleteTask_WhenTaskAlreadyCompleted_DoesNotChangeState


    // reopen tests

    // ReopenTask_WhenTaskExists_ReturnsOk
    // ReopenTask_WhenTaskExists_UpdatesStatusToTodo
    // ReopenTask_WhenTaskExists_ClearsCompletedDate
    // ReopenTask_WhenTaskDoesNotExist_ReturnsNotFound
    // ReopenTask_WhenTaskIsNotCompleted_DoesNotChangeState




    // CreateTask_ThenGetTasks_ReturnsCreatedTask
    // CompleteTask_ThenGetTaskById_ReturnsUpdatedStatus
    // ReopenTask_ThenGetTaskById_ReturnsUpdatedStatus










}