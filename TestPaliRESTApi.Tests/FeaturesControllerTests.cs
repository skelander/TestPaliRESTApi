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

    private HttpRequestMessage AuthGet(string url, string token)
    {
        var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return req;
    }

    [Fact]
    public async Task GetAll_WithToken_ReturnsOk()
    {
        var token = await GetAdminTokenAsync();
        var response = await _client.SendAsync(AuthGet("/features", token));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_WithToken_ReturnsJsonContentType()
    {
        var token = await GetAdminTokenAsync();
        var response = await _client.SendAsync(AuthGet("/features", token));

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetAll_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/features");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("1", true)]
    [InlineData("2", false)]
    public async Task SetUser_ThenGetAll_ReflectsChange(string user, bool enabled)
    {
        var token = await GetAdminTokenAsync();

        var putReq = new HttpRequestMessage(HttpMethod.Put, $"/features/{user}");
        putReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        putReq.Content = JsonContent.Create(new { Enabled = enabled });
        await _client.SendAsync(putReq);

        var flags = await (await _client.SendAsync(AuthGet("/features", token)))
            .Content.ReadFromJsonAsync<Dictionary<string, bool>>();

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
