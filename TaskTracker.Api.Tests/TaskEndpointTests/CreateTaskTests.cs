using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;

namespace TaskTracker.Api.Tests.TaskEndpointTests;

public class CreateTaskTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CreateTaskTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateTask_WhenTitleIsMissing_ReturnsBadRequest()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new
        {
            Notes = "Updated notes"
        };
    
        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);
    
        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)] // null
    [InlineData("")] // empty
    [InlineData(" ")] // whitespace
    public async Task CreateTask_WhenTitleIsNullEmptyOrWhitespace_ReturnsBadRequest(string? title)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = title!,
            Notes = "Some note"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(101)]
    [InlineData(150)]
    [InlineData(300)]
    public async Task CreateTask_WhenTitleExceeds100Characters_ReturnsBadRequest(int titleLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = new string('a', titleLength),
            Notes = "From the store"
        };
    
        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(501)]
    [InlineData(750)]
    [InlineData(1000)]
    public async Task CreateTask_WhenNotesExceeds500Characters_ReturnsBadRequest(int notesLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = new string('a', notesLength)
        };
    
        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]    
    public async Task CreateTask_WhenTitleIs1To100Characters_ReturnsCreated(int titleLength)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = new string('a', titleLength),
            Notes = "From the store"
        };
    
        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();

        createdTask.Title.Should().HaveLength(titleLength);
    }

    [Fact]
    public async Task CreateTask_WithValidRequest_ReturnsCreatedTaskResponse()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();

        createdTask.Id.Should().NotBeEmpty();
        createdTask.Title.Should().Be(createRequest.Title);
        createdTask.Notes.Should().Be(createRequest.Notes);
        createdTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());        
    }

    [Fact]
    public async Task CreateTask_WithValidRequest_PersistsTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();
        
        createdTask.Should().NotBeNull();        

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();

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

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();

        createdTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
    }

    [Fact]
    public async Task CreateTask_SetsCreatedDate()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        var timeBeforeCreate = DateTimeOffset.UtcNow;

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        var timeAfterCreate = DateTimeOffset.UtcNow;

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();

        createdTask.CreatedAt.Should().BeOnOrAfter(timeBeforeCreate);
        createdTask.CreatedAt.Should().BeOnOrBefore(timeAfterCreate);
    }

    [Fact]
    public async Task CreateTask_SetsCompletedAtToNull()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();
        
        createdTask.CompletedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateTask_ReturnsLocationHeaderForCreatedResource()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync("/tasks", createRequest);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createResponse.Headers.Location.Should().NotBeNull();

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();

        createResponse.Headers.Location.ToString().Should().Be($"/tasks/{createdTask!.Id}");
    }

} 