# TestPaliRESTApi

ASP.NET Core REST API that checks whether a string is a palindrome, with auth, feature flags, and logging.

## Tech Stack
- .NET 10
- ASP.NET Core MVC (controllers)
- EF Core InMemory (user storage)
- Microsoft.AspNetCore.Authentication.JwtBearer
- xUnit + `WebApplicationFactory` for testing
- `Microsoft.AspNetCore.Mvc.Testing` for HTTP integration tests

## Project Structure
```
TestPaliRESTApi/
  Controllers/   PalindromeController, AuthController, FeaturesController, LogsController
  Services/      IPalindromeService, IAuthService, IFeaturesService + implementations, LogStore
  Logging/       InMemoryLoggerProvider (captures TestPaliRESTApi.* logs into LogStore)
  Models/        PalindromeResult, User, LogEntry
  Data/          AppDbContext (EF Core InMemory)
  Program.cs     DI registration, JWT auth setup, user seeding

TestPaliRESTApi.Tests/
  PalindromeServiceTests.cs     # Unit tests for palindrome logic
  PalindromeControllerTests.cs  # HTTP integration tests
  AuthControllerTests.cs        # Login / JWT token tests
  FeaturesControllerTests.cs    # Feature flag endpoint tests (incl. auth)
  LogsControllerTests.cs        # Logs endpoint auth tests
```

## API Endpoints
- `POST /auth/login` — public, returns JWT token
- `GET /palindrome?input=…` — public
- `GET /features` — requires JWT (any role)
- `PUT /features/{user}` — requires JWT (admin role)
- `GET /logs` — requires JWT (admin role)

## Architecture
- Controllers depend on interfaces only, never concrete types
- JWT: HMAC-SHA256, 8-hour expiry, key from `Jwt:Key` config (env var in production)
- `UseAuthentication()` before `UseAuthorization()` in middleware pipeline
- Feature flags: in-memory `ConcurrentDictionary` singleton (resets on restart)
- Logs: `ConcurrentQueue` singleton, max 200 entries

## Code Style
- Use C# primary constructors
- Nullable enabled — service accepts `string?`, treats null as empty string
- Structured logging with message templates (not string interpolation)

## Testing Conventions
- xUnit `[Theory]` + `[InlineData]` with explicit `bool expected` parameter for palindrome tests
- Tests grouped by input type (Words, SequentialCharacters, MixedAlphanumeric, Phrases, EdgeCases)
- Auth-required endpoints: authenticate first via `POST /auth/login`, send `Bearer` token
- Run tests: `dotnet test TestPaliRESTApi.sln`
- dotnet is at `C:\Program Files\dotnet\dotnet.exe` (not on PATH in bash — use PowerShell or full path)

## Seeded Users
| Username | Password | Role  |
|----------|----------|-------|
| 1, 2, 3  | same     | user  |
| admin    | admin    | admin |
