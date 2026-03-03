namespace TestPaliRESTApi.Services;

public class PalindromeService : IPalindromeService
{
    public bool IsPalindrome(string? input)
    {
        string cleaned = new string((input ?? string.Empty).Where(char.IsLetterOrDigit).ToArray()).ToLower();
        return cleaned == new string(cleaned.Reverse().ToArray());
    }
}
