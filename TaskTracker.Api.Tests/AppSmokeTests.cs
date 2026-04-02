// using System.Net;
// using System.Net.Http.Json;
// using Microsoft.AspNetCore.Mvc.Testing;

// namespace TaskTracker.Api.Tests;

// public class AppSmokeTests : IClassFixture<WebApplicationFactory<Program>>
// {
//     private readonly HttpClient _client;

//     public AppSmokeTests(WebApplicationFactory<Program> factory)
//     {
//         _client = factory.CreateClient();
//     }

//     [Fact]
//     public async Task Root_ReturnsRunningMessage()
//     {
//         var response = await _client.GetAsync("/");
//         response.EnsureSuccessStatusCode();

//         var body = await response.Content.ReadAsStringAsync();

//         Assert.Contains("TaskTracker API is running", body);
//     }

//     [Fact]
//     public async Task Health_ReturnsHealthy()
//     {
//         var response = await _client.GetAsync("/health");

//         Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//         var body = await response.Content.ReadAsStringAsync();

//         Assert.Contains("healthy", body);
//     }
// }