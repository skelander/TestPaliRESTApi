# TestPaliRESTApi

ASP.NET Core REST API that checks whether a string is a palindrome.

## Tech Stack
- .NET 10
- ASP.NET Core MVC (controllers)
- xUnit + `WebApplicationFactory` for testing
- `Microsoft.AspNetCore.Mvc.Testing` for HTTP integration tests

## Project Structure
```
TestPaliRESTApi/
  Controllers/         # HTTP layer — depends on interfaces only
  Models/              # Response types (PalindromeResult record)
  Services/            # IPalindromeService interface + PalindromeService implementation
  Program.cs           # DI registration, exposes `public partial class Program` for tests

TestPaliRESTApi.Tests/
  PalindromeServiceTests.cs    # Unit tests for service logic
  PalindromeControllerTests.cs # HTTP integration tests via WebApplicationFactory
```

## Architecture
- Controllers depend on interfaces (`IPalindromeService`), never concrete types
- DI registered as `AddSingleton<IPalindromeService, PalindromeService>()`
- Responses use typed records (`PalindromeResult`), not anonymous objects

## Code Style
- Use C# primary constructors
- Nullable enabled — service accepts `string?`, treats null as empty string

## Testing Conventions
- xUnit `[Theory]` + `[InlineData]` with explicit `bool expected` parameter on every case (no implicit assertion methods like `Assert.True/False`)
- Tests grouped by input type, not by expected result:
  - `IsPalindrome_Words`
  - `IsPalindrome_SequentialCharacters`
  - `IsPalindrome_MixedAlphanumeric`
  - `IsPalindrome_Phrases`
  - `IsPalindrome_EdgeCases`
- Run tests: `dotnet test TestPaliRESTApi.sln`
- dotnet is at `C:\Program Files\dotnet\dotnet.exe` (not on PATH in bash — use PowerShell or full path)
