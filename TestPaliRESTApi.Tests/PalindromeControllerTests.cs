using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace TestPaliRESTApi.Tests;

public class PalindromeControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PalindromeControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("racecar", true)]
    [InlineData("hello", false)]
    public async Task Check_ReturnsCorrectResult(string input, bool expectedIsPalindrome)
    {
        var response = await _client.GetAsync($"/palindrome?input={input}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PalindromeResponse>();
        Assert.NotNull(body);
        Assert.Equal(input, body.Input);
        Assert.Equal(expectedIsPalindrome, body.IsPalindrome);
    }

    [Fact]
    public async Task Check_PhraseWithSpaces_ReturnsPalindrome()
    {
        var response = await _client.GetAsync("/palindrome?input=A%20man%20a%20plan%20a%20canal%20Panama");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<PalindromeResponse>();
        Assert.NotNull(body);
        Assert.True(body.IsPalindrome);
        Assert.Equal("A man a plan a canal Panama", body.Input);
    }

    [Fact]
    public async Task Check_ReturnsJsonContentType()
    {
        var response = await _client.GetAsync("/palindrome?input=racecar");

        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
    }

    [Theory]
    [InlineData("racecar", "\"racecar\" is a palindrome.")]
    [InlineData("hello", "\"hello\" is not a palindrome.")]
    public async Task Check_ReturnsCorrectMessage(string input, string expectedMessage)
    {
        var response = await _client.GetAsync($"/palindrome?input={input}");

        var body = await response.Content.ReadFromJsonAsync<PalindromeResponse>();
        Assert.NotNull(body);
        Assert.Equal(expectedMessage, body.Message);
    }

    [Fact]
    public async Task Check_MissingInput_Returns400()
    {
        var response = await _client.GetAsync("/palindrome");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Check_PostMethod_Returns405()
    {
        var response = await _client.PostAsync("/palindrome?input=racecar", null);

        Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
    }

    [Fact]
    public async Task Check_PreservesOriginalCasingInResponse()
    {
        var response = await _client.GetAsync("/palindrome?input=RaceCar");

        var body = await response.Content.ReadFromJsonAsync<PalindromeResponse>();
        Assert.NotNull(body);
        Assert.Equal("RaceCar", body.Input);
        Assert.True(body.IsPalindrome);
    }

    private record PalindromeResponse(string Input, bool IsPalindrome, string Message);
}
