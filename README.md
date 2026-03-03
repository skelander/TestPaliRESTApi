# TestPaliRESTApi

A simple ASP.NET Core REST API that checks whether a string is a palindrome.

## Endpoint

### `GET /palindrome/{input}`

Checks if the given string is a palindrome. Non-alphanumeric characters (spaces, punctuation) are ignored and the comparison is case-insensitive.

**Example request:**
```
GET /palindrome/racecar
```

**Example response:**
```json
{
  "input": "racecar",
  "isPalindrome": true,
  "message": "\"racecar\" is a palindrome."
}
```

**More examples:**

| Input | isPalindrome |
|-------|-------------|
| `racecar` | `true` |
| `RaceCar` | `true` |
| `A man a plan a canal Panama` | `true` |
| `Was it a car or a cat I saw?` | `true` |
| `hello` | `false` |

**Edge cases:**
- Empty string, whitespace-only, and null → `true` (empty string is considered a palindrome)
- Punctuation-only (e.g. `!!!`) → `true` (strips to empty)

## Getting Started

### Prerequisites
- .NET 10 SDK

### Run the API
```
dotnet run --project TestPaliRESTApi
```

### Run tests
```
dotnet test TestPaliRESTApi.sln
```

## Project Structure

```
TestPaliRESTApi/
  Controllers/    PalindromeController — HTTP routing, returns PalindromeResult
  Models/         PalindromeResult — typed response record
  Services/       IPalindromeService interface + PalindromeService implementation

TestPaliRESTApi.Tests/
  PalindromeServiceTests.cs    — unit tests for palindrome logic
  PalindromeControllerTests.cs — HTTP integration tests
```

## Architecture

The project follows a clean architecture approach:

- **Controller** depends on `IPalindromeService` (interface), not the concrete implementation
- **PalindromeService** contains all palindrome logic and implements `IPalindromeService`
- **DI** is registered in `Program.cs` as `AddSingleton<IPalindromeService, PalindromeService>()`
- **Responses** use the typed `PalindromeResult` record

This separation means the service logic can be unit tested directly, and the controller can be tested via the full HTTP pipeline using `WebApplicationFactory`.

## Documentation

- [Architecture](docs/architecture.md) — layers, design decisions, and rationale
- [Testing Strategy](docs/testing-strategy.md) — test structure, grouping conventions, and edge case reasoning

## Tests

32 tests across two files:

**PalindromeServiceTests** — pure logic tests, grouped by input type:
- `IsPalindrome_Words` — single words, mixed case, near-misses
- `IsPalindrome_SequentialCharacters` — numeric and alphabetic sequences
- `IsPalindrome_MixedAlphanumeric` — combined letters and digits
- `IsPalindrome_Phrases` — multi-word strings with spaces and punctuation
- `IsPalindrome_EdgeCases` — empty string, null, whitespace-only, punctuation-only

**PalindromeControllerTests** — HTTP-level tests:
- Correct `isPalindrome` result and response shape
- Correct message text for palindrome and non-palindrome
- `application/json` content type
- URL-decoded input preserved in response
- Original casing preserved in response
- `GET /palindrome` (missing segment) → 404
- `POST /palindrome/racecar` → 405 Method Not Allowed
