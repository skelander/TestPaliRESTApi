using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestPaliRESTApi.Tests;

public class FeaturesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public FeaturesControllerTests(WebApplicationFactory<Program> factory)
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
    public async Task GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/features");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsJsonContentType()
    {
        var response = await _client.GetAsync("/features");

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("2", false)]
    public async Task SetUser_ThenGetAll_ReflectsChange(string user, bool enabled)
    {
        var token = await GetAdminTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Put, $"/features/{user}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { Enabled = enabled });
        await _client.SendAsync(request);

        var response = await _client.GetAsync("/features");
        var flags = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();

        Assert.NotNull(flags);
        Assert.Equal(enabled, flags[user]);
    }

    [Fact]
    public async Task SetUser_ReturnsOk()
    {
        var token = await GetAdminTokenAsync();
        var request = new HttpRequestMessage(HttpMethod.Put, "/features/1");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { Enabled = false });
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task SetUser_WithoutToken_Returns401()
    {
        var response = await _client.PutAsJsonAsync("/features/1", new { Enabled = false });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
