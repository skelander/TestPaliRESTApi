using TestPaliRESTApi.Services;
using Xunit;

namespace TestPaliRESTApi.Tests;

public class PalindromeServiceTests
{
    private readonly PalindromeService _sut = new();

    [Theory]
    [InlineData("racecar", true)]
    [InlineData("RaceCar", true)]
    [InlineData("level", true)]
    [InlineData("deified", true)]
    [InlineData("hello", false)]
    [InlineData("palindrome", false)]
    [InlineData("openai", false)]
    [InlineData("software", false)]
    [InlineData("almost", false)]
    [InlineData("racecat", false)]
    [InlineData("racecars", false)]
    [InlineData("ab", false)]
    public void IsPalindrome_Words(string input, bool expected)
    {
        Assert.Equal(expected, _sut.IsPalindrome(input));
    }

    [Theory]
    [InlineData("12321", true)]
    [InlineData("12345", false)]
    [InlineData("abcde", false)]
    public void IsPalindrome_SequentialCharacters(string input, bool expected)
    {
        Assert.Equal(expected, _sut.IsPalindrome(input));
    }

    [Theory]
    [InlineData("1a1", true)]
    [InlineData("a1b", false)]
    public void IsPalindrome_MixedAlphanumeric(string input, bool expected)
    {
        Assert.Equal(expected, _sut.IsPalindrome(input));
    }

    [Theory]
    [InlineData("A man a plan a canal Panama", true)]
    [InlineData("Was it a car or a cat I saw?", true)]
    [InlineData("Not a palindrome at all", false)]
    public void IsPalindrome_Phrases(string input, bool expected)
    {
        Assert.Equal(expected, _sut.IsPalindrome(input));
    }

    [Theory]
    [InlineData("a", true)]
    [InlineData("", true)]
    [InlineData(null, true)]
    [InlineData("   ", true)]
    [InlineData("!!!", true)]
    public void IsPalindrome_EdgeCases(string? input, bool expected)
    {
        Assert.Equal(expected, _sut.IsPalindrome(input));
    }
}
