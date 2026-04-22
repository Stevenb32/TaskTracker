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

    [Fact]
    public async Task CreateTask_ReturnsLocationHeaderForCreatedResource()
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
        response.Headers.Location.Should().NotBeNull();

        var createdTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();

        response.Headers.Location.ToString().Should().Be($"/tasks/{createdTask!.Id}");
    }

    [Theory]
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    public async Task CreateTask_WhenTitleIsNullEmptyOrWhitespace_ReturnsBadRequest(string? title)
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

    [Fact]
    public async Task CompleteTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();        

        // When
        var response = await _client.PostAsync($"/tasks/{nonExistentId}/complete", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);        
    }

    [Fact]
    public async Task CompleteTask_WhenTaskAlreadyCompleted_DoesNotChangeState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        var firstCompleteResponse = await _client.PostAsync($"/tasks/{task.Id}/complete", null);
        firstCompleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var firstCompletedTask = await firstCompleteResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        firstCompletedTask.Should().NotBeNull();

        // When
        var secondCompleteResponse = await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // Then
        secondCompleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondCompletedTask = await secondCompleteResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        secondCompletedTask.Should().NotBeNull();

        secondCompletedTask!.Id.Should().Be(firstCompletedTask!.Id);
        secondCompletedTask.Title.Should().Be(firstCompletedTask.Title);
        secondCompletedTask.Notes.Should().Be(firstCompletedTask.Notes);
        secondCompletedTask.Status.Should().Be(firstCompletedTask.Status);
        secondCompletedTask.CreatedAt.Should().Be(firstCompletedTask.CreatedAt);
        secondCompletedTask.CompletedAt.Should().Be(firstCompletedTask.CompletedAt);

        var savedTask = await _factory.GetTaskByIdAsync(task.Id);
        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(firstCompletedTask.Id);
        savedTask.Title.Should().Be(firstCompletedTask.Title);
        savedTask.Notes.Should().Be(firstCompletedTask.Notes);
        savedTask.Status.Should().Be(Domain.TaskStatus.Completed);
        savedTask.CreatedAt.Should().Be(firstCompletedTask.CreatedAt);
        savedTask!.CompletedAt.Should().Be(firstCompletedTask.CompletedAt);
    }
    #endregion

    // =============================================================================================================================== 
    #region Reopen Tests
    // =============================================================================================================================== 

    // 
    [Fact]
    public async Task ReopenTask_WhenTaskExists_ReturnsOk()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask.Id.Should().Be(task.Id);
    }

    [Fact]
    public async Task ReopenTask_WhenTaskExists_UpdatesStatusToActive()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask.Id.Should().Be(task.Id);
        responseTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
    }

    [Fact]
    public async Task ReopenTask_WhenTaskExists_ClearsCompletedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask.Id.Should().Be(task.Id);
        responseTask.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task ReopenTask_WhenTaskDoesNotExist_ReturnsNotFound()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var nonExistentId = Guid.NewGuid();        

        // When        
        var response = await _client.PostAsync($"/tasks/{nonExistentId}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReopenTask_WhenTaskIsNotCompleted_DoesNotChangeState()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));        

        await _factory.AddTaskAsync(task);

        // When        
        var response = await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        responseTask.Should().NotBeNull();
        responseTask!.Id.Should().Be(task.Id);
        responseTask.Status.Should().Be(task.Status.ToString());
        responseTask.CompletedAt.Should().Be(task.CompletedAt);

        var savedTask = await _factory.GetTaskByIdAsync(task.Id);
        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(task.Id);
        savedTask.Title.Should().Be(task.Title);
        savedTask.Notes.Should().Be(task.Notes);
        savedTask.Status.Should().Be(task.Status);
        savedTask.CreatedAt.Should().Be(task.CreatedAt);
        savedTask.CompletedAt.Should().Be(task.CompletedAt);
    }
    #endregion


    // =============================================================================================================================== 
    #region Integration Tests
    // =============================================================================================================================== 
    [Fact]
    public async Task CreateTask_ThenGetTask_ReturnsCreatedTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var request = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", request);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        createdTask.Should().NotBeNull();

        var getByIdResponse = await _client.GetAsync($"/tasks/{createdTask.Id}");
        
        // Then
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await getByIdResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(createdTask.Id);
        retrievedTask.Title.Should().Be(createdTask.Title);
        retrievedTask.Notes.Should().Be(createdTask.Notes);
        retrievedTask.Status.Should().Be(createdTask.Status);
        retrievedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        retrievedTask.CompletedAt.Should().Be(createdTask.CompletedAt);
    }

    [Fact]
    public async Task CompleteTask_ThenGetTaskById_ReturnsUpdatedStatus()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);

        // When
        var response = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(task.Id);
        retrievedTask.Status.Should().Be(Domain.TaskStatus.Completed.ToString());
        retrievedTask.CompletedAt.Should().NotBeNull(); 
    }
 
    [Fact]
    public async Task ReopenTask_ThenGetTaskById_ReturnsUpdatedStatus()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);
        await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // When
        
        var response = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(task.Id);
        retrievedTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());        
    }

    [Fact]
    public async Task ReopenTask_ThenGetTaskById_ClearsCompletedAt()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var task = TaskItem.Create("Buy milk", "From the store", new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        await _client.PostAsync($"/tasks/{task.Id}/complete", null);
        await _client.PostAsync($"/tasks/{task.Id}/reopen", null);

        // When
        
        var response = await _client.GetAsync($"/tasks/{task.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var retrievedTask = await response.Content.ReadFromJsonAsync<TaskItemResponse>();
        retrievedTask.Should().NotBeNull();

        retrievedTask.Id.Should().Be(task.Id);
        retrievedTask.CompletedAt.Should().BeNull();        
    }
    #endregion
}