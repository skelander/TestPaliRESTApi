# Architecture

## Overview

The API follows a layered pattern. Controllers handle HTTP concerns only; services hold business logic; models define data shapes. Controllers depend on interfaces, never concrete types.

## Controllers

| Controller | Endpoints | Auth required |
|------------|-----------|---------------|
| `AuthController` | `POST /auth/login` | None |
| `PalindromeController` | `GET /palindrome?input=…` | None |
| `FeaturesController` | `GET /features` / `PUT /features/{user}` | GET: any JWT; PUT: admin JWT |
| `LogsController` | `GET /logs` | Admin JWT |

## Authentication

`POST /auth/login` validates credentials against the EF Core in-memory database and returns a signed JWT (HMAC-SHA256, 8-hour expiry). The JWT signing key is injected at runtime from an environment variable (Fly.io secret in production; `appsettings.json` fallback for development).

Protected endpoints use `[Authorize(Roles = "admin")]`. ASP.NET Core's JWT bearer middleware validates the token on every request.

## Services

| Service | Lifetime | Notes |
|---------|----------|-------|
| `PalindromeService` | Singleton | Stateless — safe for shared use |
| `FeaturesService` | Singleton | In-memory `ConcurrentDictionary`; resets on restart |
| `AuthService` | Scoped | Uses `AppDbContext` |
| `LogStore` | Singleton | `ConcurrentQueue`, capped at 200 entries |

## Logging

A custom `InMemoryLoggerProvider` captures log entries from `TestPaliRESTApi.*` categories (Information and above) into `LogStore`. The `GET /logs` endpoint serves these entries to the admin frontend, which polls every 5 seconds.

## Key Decisions

### Interface over concrete type
Controllers inject interfaces (`IPalindromeService`, `IFeaturesService`, etc.), not concrete classes. This decouples the HTTP layer from implementations and makes testing with mocks straightforward.

### Query parameter instead of path segment
The palindrome endpoint uses `GET /palindrome?input=…` rather than `GET /palindrome/{input}`. A dot (`.`) in a URL path segment causes ASP.NET Core routing to fail; a query parameter avoids this entirely.

### `public partial class Program`
`Program.cs` ends with `public partial class Program { }`, which exposes the entry point to `WebApplicationFactory<Program>` in the test project — enabling full integration tests without test-specific configuration.
