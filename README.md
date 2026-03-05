# Palindrome

A palindrome checker built with ASP.NET Core. A REST API handles logic and auth; a static frontend is hosted on GitHub Pages.

**Live:** https://skelander.github.io/TestPaliRESTApi/login.html

---

## API Endpoints

### `POST /auth/login`
Validates credentials. Returns the username, role, and a signed JWT token (8-hour expiry), or 401.

```json
{ "username": "1", "password": "1" }
→ { "user": "1", "role": "user", "token": "<jwt>" }
```

### `GET /palindrome?input=…`
Checks if the input is a palindrome. Whitespace is ignored; everything else (punctuation, symbols) is kept and compared.

```
GET /palindrome?input=racecar
→ { "input": "racecar", "isPalindrome": true, "message": "\"racecar\" is a palindrome." }
```

| Input | isPalindrome |
|-------|-------------|
| `racecar` | `true` |
| `RaceCar` | `true` |
| `A man a plan a canal Panama` | `true` |
| `hello` | `false` |
| `dsdsd%%` | `false` |

### `GET /features`
Returns the current API access flag per user.

### `PUT /features/{user}`
Enables or disables API access for a specific user. Requires admin JWT (`Authorization: Bearer <token>`).

```json
{ "enabled": false }
```

### `GET /logs`
Returns recent application log entries (up to 200). Requires admin JWT.

---

## Test accounts

| Username | Password | Role |
|----------|----------|------|
| `1`, `2`, `3` | same as username | user |
| `admin` | `admin` | admin |

---

## Tech stack

- .NET 10 / ASP.NET Core MVC
- EF Core (in-memory database for user storage)
- xUnit + `WebApplicationFactory`
- Docker + Fly.io (API)
- GitHub Pages (frontend)
- GitHub Actions (CI/CD)

---

## Project structure

```
frontend/                        static HTML/CSS/JS (deployed to GitHub Pages)
  login.html, index.html, admin.html, about.html, how-we-did-it.html

TestPaliRESTApi/
  Controllers/   PalindromeController, AuthController, FeaturesController, LogsController
  Services/      IPalindromeService, IAuthService, IFeaturesService + implementations, LogStore
  Logging/       InMemoryLoggerProvider (captures app logs to LogStore)
  Models/        PalindromeResult, User, LogEntry
  Data/          AppDbContext
  Program.cs     DI registration, JWT auth, user seeding

TestPaliRESTApi.Tests/
  PalindromeServiceTests.cs     unit tests for palindrome logic
  PalindromeControllerTests.cs  HTTP integration tests
  AuthControllerTests.cs        login and JWT token tests
  FeaturesControllerTests.cs    feature flag endpoint tests (incl. auth)
  LogsControllerTests.cs        logs endpoint auth tests
```

---

## Running locally

```
dotnet run --project TestPaliRESTApi
dotnet test TestPaliRESTApi.sln
```

---

## CI/CD

Two GitHub Actions workflows trigger on push to `main`:

- **api.yml** — build → test → deploy to Fly.io (on `.cs`/`.csproj`/`Dockerfile`/`fly.toml` changes)
- **frontend.yml** — deploy to GitHub Pages (on `frontend/**` changes); injects `API_URL` repo variable at build time

A failing test blocks deployment.
