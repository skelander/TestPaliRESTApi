using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestPaliRESTApi.Tests;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("1", "1", "user")]
    [InlineData("2", "2", "user")]
    [InlineData("3", "3", "user")]
    [InlineData("admin", "admin", "admin")]
    public async Task Login_ValidCredentials_ReturnsOkWithRole(string username, string password, string expectedRole)
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new { Username = username, Password = password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(body);
        Assert.Equal(username, body.User);
        Assert.Equal(expectedRole, body.Role);
        Assert.False(string.IsNullOrEmpty(body.Token));
    }

    [Theory]
    [InlineData("1", "wrong")]
    [InlineData("unknown", "anything")]
    [InlineData("admin", "1")]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized(string username, string password)
    {
        var response = await _client.PostAsJsonAsync("/auth/login", new { Username = username, Password = password });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private record LoginResponse(string User, string Role, string Token);
}
