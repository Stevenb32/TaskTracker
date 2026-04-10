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
        createdTask.Title.Should().Be(request.Title);
        createdTask.Notes.Should().Be(request.Notes);
        createdTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());        
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
        createdTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
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

    [Fact]
    public async Task GetTasks_WhenTasksExist_ReturnsOkWithTasks()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task1 = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));
        var task2 = TaskItem.Create("Walk dog", "Around the block", new DateTimeOffset(2026, 3, 25, 8, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task1);
        await _factory.AddTaskAsync(task2);

        // When        
        var response = await _client.GetAsync("/tasks");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();
        tasks.Should().NotBeNull();
        tasks.Should().HaveCount(2);

        tasks.Should().Contain(t => t.Title == task1.Title && t.Notes == task1.Notes);
        tasks.Should().Contain(t => t.Title == task2.Title && t.Notes == task2.Notes);
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

    #endregion


    // =============================================================================================================================== 
    #region Complete Tests
    // =============================================================================================================================== 

    [Fact]
    public async Task CompleteTask_WhenTaskExists_ReturnsOk()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();

        responseTask.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task CompleteTask_WhenTaskExists_UpdatesStatusToCompleted()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
    
        responseTask.Id.Should().Be(task.Id);
        responseTask.Status.Should().Be(Domain.TaskStatus.Completed.ToString());
    }

    [Fact]
    public async Task CompleteTask_WhenTaskExists_SetsCompletedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));                

        await _factory.AddTaskAsync(task);        

        // When        
        var timeBefore = DateTimeOffset.UtcNow;
        var response = await _client.PostAsync($"/tasks/{task.Id}/complete", null);
        var timeAfter = DateTimeOffset.UtcNow;

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        
        responseTask.CompletedAt.Should().NotBeNull();
        responseTask.CompletedAt.Should().BeOnOrAfter(timeBefore);
        responseTask.CompletedAt.Should().BeOnOrBefore(timeAfter);       
    }

    [Fact]
    public async Task CompleteTask_WhenTaskExists_PersistsCompletedState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        // When
        var response = await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var completedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        completedTask.Should().NotBeNull();

        var savedTask = await _factory.GetTaskByIdAsync(task.Id);

        savedTask.Should().NotBeNull();
        savedTask.Id.Should().Be(task.Id);
        savedTask.Status.Should().Be(Domain.TaskStatus.Completed);
        savedTask.CompletedAt.Should().NotBeNull();

        savedTask.Id.Should().Be(completedTask.Id);
    }




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