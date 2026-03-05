using Microsoft.AspNetCore.Mvc;
using TestPaliRESTApi.Models;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PalindromeController(IPalindromeService service, ILogger<PalindromeController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Check([FromQuery] string input)
    {
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
