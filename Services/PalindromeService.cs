namespace TestPaliRESTApi.Services;

public class PalindromeService : IPalindromeService
{
    public bool IsPalindrome(string? input)
    {
        string cleaned = new string((input ?? string.Empty)
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray())
            .ToLower();
        return cleaned == new string(cleaned.Reverse().ToArray());
    }
}
