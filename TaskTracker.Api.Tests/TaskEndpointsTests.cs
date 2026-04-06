using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TaskTracker.Api.Dtos;
using TaskTracker.Domain;


namespace TaskTracker.Api.Tests;

public class TaskEndpointsTests : IClassFixture<TaskTrackerWebApplicationFactory>
{
    private readonly TaskTrackerWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TaskEndpointsTests(TaskTrackerWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ReturnsOk()
    {
        var response = await _client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }










    // custom factory tests
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

        var task = TaskItem.Create(
            "Buy milk",
            "From the store",
            new DateTimeOffset(2026, 3, 25, 7, 0, 0, TimeSpan.Zero));

        await _factory.AddTaskAsync(task);

        // When
        var response = await _client.GetAsync("/tasks");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var tasks = await response.Content.ReadFromJsonAsync<List<TaskItemResponse>>();
        tasks.Should().NotBeNull();
        tasks.Should().HaveCount(1);
        tasks![0].Title.Should().Be("Buy milk");
    }













}