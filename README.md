# Palindrome

A palindrome checker built with ASP.NET Core. A REST API handles logic and auth; a static frontend is hosted on GitHub Pages.

**Live:** https://skelander.github.io/TestPaliRESTApi/login.html

---

## API Endpoints

### `POST /auth/login`
Validates credentials. Returns the user's username and role, or 401.

```json
{ "username": "1", "password": "1" }
→ { "user": "1", "role": "user" }
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
Enables or disables API access for a specific user (admin only via the frontend).

```json
{ "enabled": false }
```

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
  login.html, index.html, admin.html, about.html

TestPaliRESTApi/
  Controllers/   PalindromeController, AuthController, FeaturesController
  Services/      IPalindromeService, IAuthService, IFeaturesService + implementations
  Models/        PalindromeResult, User
  Data/          AppDbContext
  Program.cs     DI registration, user seeding

TestPaliRESTApi.Tests/
  PalindromeServiceTests.cs     unit tests for palindrome logic
  PalindromeControllerTests.cs  HTTP integration tests
  AuthControllerTests.cs        login endpoint tests
  FeaturesControllerTests.cs    feature flag endpoint tests
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
