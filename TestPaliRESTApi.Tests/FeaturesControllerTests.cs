using System.Net;
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
        await _client.PutAsJsonAsync($"/features/{user}", new { Enabled = enabled });

        var response = await _client.GetAsync("/features");
        var flags = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();

        Assert.NotNull(flags);
        Assert.Equal(enabled, flags[user]);
    }

    [Fact]
    public async Task SetUser_ReturnsOk()
    {
        var response = await _client.PutAsJsonAsync("/features/1", new { Enabled = false });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
