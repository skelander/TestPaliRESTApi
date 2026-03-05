using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestPaliRESTApi.Tests;

public class LogsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public LogsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> GetAdminTokenAsync()
    {
        var res = await _client.PostAsJsonAsync("/auth/login", new { Username = "admin", Password = "admin" });
        var data = await res.Content.ReadFromJsonAsync<LoginData>();
        return data!.Token;
    }

    private record LoginData(string User, string Role, string Token);

    [Fact]
    public async Task GetLogs_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/logs");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetLogs_WithAdminToken_Returns200()
    {
        var token = await GetAdminTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Get, "/logs");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
