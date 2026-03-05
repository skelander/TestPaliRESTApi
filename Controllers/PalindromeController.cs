using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Models;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PalindromeController(IPalindromeService service, ILogger<PalindromeController> logger) : ControllerBase
{
    private const int MaxInputLength = 1000;

    [HttpGet]
    public IActionResult Check([FromQuery] string? input)
    {
        if (input is null) return BadRequest("Input is required.");
        if (input.Length > MaxInputLength)
        {
            logger.LogWarning("Palindrome check rejected: input length {Length} exceeds limit {Limit}", input.Length, MaxInputLength);
            return BadRequest($"Input must not exceed {MaxInputLength} characters.");
        }

        bool isPalindrome = service.IsPalindrome(input);
        logger.LogInformation("Palindrome check: input={Input} result={IsPalindrome}", input, isPalindrome);

        return Ok(new PalindromeResult(
            Input: input,
            IsPalindrome: isPalindrome,
            Message: isPalindrome
                ? $"\"{input}\" is a palindrome."
                : $"\"{input}\" is not a palindrome."
        ));
    }
}
