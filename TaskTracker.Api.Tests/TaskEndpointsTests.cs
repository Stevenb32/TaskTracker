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


    // =============================================================================================================================== 
    #region Create Tests
    // =============================================================================================================================== 
    [Fact]
    public async Task CreateTask_WithValidRequest_ReturnsCreatedTaskResponse()
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
        createdTask!.Id.Should().NotBeEmpty();
        createdTask.Title.Should().Be("Buy milk");
        createdTask.Notes.Should().Be("From the store");
        createdTask.Status.Should().Be("Active");        
    }

    [Fact]
    public async Task CreateTask_WithValidRequest_PersistsTask()
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

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask!.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Status.Should().Be(Domain.TaskStatus.Active);
        savedTask.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateTask_SetsInitialStatusToActive()
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
        createdTask.Status.Should().Be("Active");
    }

    [Fact]
    public async Task CreateTask_SetsCreatedDate()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        var timeBefore = DateTimeOffset.UtcNow;

        // When
        var response = await _client.PostAsJsonAsync("/tasks", request);

        var timeAfter = DateTimeOffset.UtcNow;

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();

        createdTask.CreatedAt.Should().BeOnOrAfter(timeBefore);
        createdTask.CreatedAt.Should().BeOnOrBefore(timeAfter);
    }

    [Fact]
    public async Task CreateTask_SetsCompletedAtToNull()
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
        createdTask.CompletedAt.Should().BeNull();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]    
    public async Task CreateTask_WhenTitleIs1To100Characters_ReturnsCreated(int titleLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = new string('a', titleLength),
            Notes = "From the store"
        };
    
        // When
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();
        createdTask.Title.Should().HaveLength(titleLength);
    }

    [Theory]
    [InlineData(null)] // null
    [InlineData("")] // empty    
    public async Task CreateTask_WhenTitleIsNullOrEmpty_ReturnsBadRequest(string? title)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = title!,
            Notes = "Some note"
        };

        // When
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public async Task CreateTask_WhenTitleExceeds100Characters_ReturnsBadRequest(int titleLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = new string('a', titleLength),
            Notes = "From the store"
        };
    
        // When
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public async Task CreateTask_WhenNotesExceeds500Characters_ReturnsBadRequest(int notesLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = new string('a', notesLength)
        };
    
        // When
        var response = await _client.PostAsJsonAsync("/tasks", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    #endregion



    // =============================================================================================================================== 
    #region Get Tests
    // =============================================================================================================================== 

    [Fact]
    public async Task GetTasks_WhenNoTasksExist_ReturnsOkWithEmptyList()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        // When        
        var response = await _client.GetAsync("/tasks");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();
        tasks.Should().NotBeNull();
        tasks.Should().BeEmpty();       
    }
    // GetTasks_WhenTasksExist_ReturnsOkWithTasks







    // GetTaskById_WhenTaskExists_ReturnsOkWithTask
    // GetTaskById_WhenTaskDoesNotExist_ReturnsNotFound
    // GetTaskById_ReturnsCorrectTask


    #endregion


















    // =============================================================================================================================== 
    #region Complete Tests
    // =============================================================================================================================== 
    


    // CompleteTask_WhenTaskExists_ReturnsOk
    // CompleteTask_WhenTaskExists_UpdatesStatusToDone
    // CompleteTask_WhenTaskExists_SetsCompletedAt
    // CompleteTask_WhenTaskDoesNotExist_ReturnsNotFound
    // CompleteTask_WhenTaskAlreadyCompleted_DoesNotChangeState




    #endregion

























    // =============================================================================================================================== 
    #region Reopen Tests
    // =============================================================================================================================== 

    // ReopenTask_WhenTaskExists_ReturnsOk
    // ReopenTask_WhenTaskExists_UpdatesStatusToActive
    // ReopenTask_WhenTaskExists_ClearsCompletedAt
    // ReopenTask_WhenTaskDoesNotExist_ReturnsNotFound
    // ReopenTask_WhenTaskIsNotCompleted_DoesNotChangeState


    #endregion















    
    // =============================================================================================================================== 
    #region Integration Tests
    // =============================================================================================================================== 
    
    // CreateTask_ThenGetTasks_ReturnsCreatedTask
    // CompleteTask_ThenGetTaskById_ReturnsUpdatedStatus
    // ReopenTask_ThenGetTaskById_ReturnsUpdatedStatus





    #endregion




}