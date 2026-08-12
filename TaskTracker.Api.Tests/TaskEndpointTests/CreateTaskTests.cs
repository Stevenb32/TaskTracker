using System.Net;
using System.Net.Http.Json;
using System.Text;
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
    public async Task CreateTask_WhenPriorityIsUnknown_ReturnsBadRequest()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store",
            Priority = TaskPriority.Unknown
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("""{"title":"Buy milk","notes":"From the store","priority":2}""")]
    [InlineData("""{"title":"Buy milk","notes":"From the store","priority":99}""")]
    [InlineData("""{"title":"Buy milk","notes":"From the store","priority":"banana"}""")]
    [InlineData("""{"title":"Buy milk","notes":"From the store","priority":null}""")]
    public async Task CreateTask_WhenPriorityPayloadIsInvalid_ReturnsBadRequest(string json)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // When
        var createResponse = await _client.PostAsync("/tasks", content);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateTask_WhenTitleIsMissing_ReturnsBadRequest()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new
        {
            Notes = "From the store",
            Priority = TaskPriority.Low
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateTask_WhenTitleIsNullEmptyOrWhitespace_ReturnsBadRequest(string? title)
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = title!,
            Notes = "Some note",
            Priority = TaskPriority.Low
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

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
            Notes = "From the store",
            Priority = TaskPriority.Low
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

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
            Notes = new string('a', notesLength),
            Priority = TaskPriority.Low
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

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
            Notes = "From the store",
            Priority = TaskPriority.Low
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();
        createdTask!.Title.Should().HaveLength(titleLength);
    }

    [Fact]
    public async Task CreateTask_WithValidRequest_ReturnsCreatedResponseAndPersistsTask()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store",
            Priority = TaskPriority.Critical
        };

        var timeBeforeCreate = DateTimeOffset.UtcNow;

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        var timeAfterCreate = DateTimeOffset.UtcNow;

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();
        createdTask!.Id.Should().NotBeEmpty();
        createdTask.Title.Should().Be(createRequest.Title);
        createdTask.Notes.Should().Be(createRequest.Notes);
        createdTask.Priority.Should().Be(createRequest.Priority.ToString());
        createdTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());
        createdTask.CreatedAt.Should().BeOnOrAfter(timeBeforeCreate);
        createdTask.CreatedAt.Should().BeOnOrBefore(timeAfterCreate);
        createdTask.CompletedAt.Should().BeNull();
        createdTask.UpdatedAt.Should().BeNull();

        createResponse.Headers.Location.Should().NotBeNull();
        createResponse.Headers.Location!.ToString().Should().Be($"/tasks/{createdTask.Id}");

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Priority.Should().Be(createRequest.Priority);
        savedTask.Status.Should().Be(Domain.TaskStatus.Active);
        savedTask.CreatedAt.Should().Be(createdTask.CreatedAt);
        savedTask.CompletedAt.Should().BeNull();
        savedTask.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public async Task CreateTask_WhenPriorityIsNotProvided_UsesAndPersistsDefaultPriority()
    {
        // Given
        await _factory.ResetDatabaseAsync();

        var createRequest = new TaskItemCreateRequest
        {
            Title = "Buy milk",
            Notes = "From the store"
        };

        // When
        var createResponse = await _client.PostAsJsonAsync(
            "/tasks",
            createRequest,
            TaskTrackerWebApplicationFactory.JsonSerializerOptions);

        // Then
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskItemResponse>();

        createdTask.Should().NotBeNull();
        createdTask!.Id.Should().NotBeEmpty();
        createdTask.Title.Should().Be(createRequest.Title);
        createdTask.Notes.Should().Be(createRequest.Notes);
        createdTask.Priority.Should().Be(TaskPriority.Medium.ToString());
        createdTask.Status.Should().Be(Domain.TaskStatus.Active.ToString());

        var savedTask = await _factory.GetTaskByIdAsync(createdTask.Id);

        savedTask.Should().NotBeNull();
        savedTask!.Id.Should().Be(createdTask.Id);
        savedTask.Title.Should().Be(createdTask.Title);
        savedTask.Notes.Should().Be(createdTask.Notes);
        savedTask.Priority.Should().Be(TaskPriority.Medium);
        savedTask.Status.Should().Be(Domain.TaskStatus.Active);
        savedTask.CompletedAt.Should().BeNull();
        savedTask.UpdatedAt.Should().BeNull();
    }
}
