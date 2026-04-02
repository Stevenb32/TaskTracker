using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TaskTracker.Api.Tests;

public class TaskEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaskEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTasks_ReturnsOk()
    {
        var response = await _client.GetAsync("/tasks");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}