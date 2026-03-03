# Architecture

## Overview

The API follows a clean architecture approach with three distinct layers: HTTP, domain logic, and models. Each layer has a single responsibility and depends only on abstractions.

```
HTTP Request
     │
     ▼
PalindromeController          (Controllers/)
     │  depends on interface
     ▼
IPalindromeService            (Services/)
     │  implemented by
     ▼
PalindromeService             (Services/)
     │  returns bool to controller
     ▼
PalindromeResult              (Models/)
     │  serialized to JSON
     ▼
HTTP Response
```

## Layers

### Controllers
Responsible only for HTTP concerns: routing, reading the request, and returning the response. Contains no business logic. Depends on `IPalindromeService`, never on `PalindromeService` directly.

### Services
Contains the palindrome logic. `IPalindromeService` defines the contract; `PalindromeService` implements it. This separation means the controller can be tested with a mock or stub service, and the service can be tested without an HTTP layer.

### Models
`PalindromeResult` is a typed record representing the API response. Using a named type instead of an anonymous object makes the response shape explicit and reusable.

## Key Decisions

### Interface over concrete type
The controller injects `IPalindromeService`, not `PalindromeService`. This decouples the HTTP layer from the implementation, making it straightforward to:
- Swap the implementation (e.g. a cached or async version)
- Mock the service in tests without spinning up the real logic

### Singleton lifetime
`PalindromeService` is registered as a singleton because it holds no state — the same instance can safely serve all requests.

### Null handling in the service
The service accepts `string?` and treats null as an empty string via `?? string.Empty`. An empty string cleans to `""`, which is considered a palindrome. This keeps the controller simple — it passes whatever it receives without null-checking.

### `public partial class Program`
`Program.cs` ends with `public partial class Program { }`. This exposes the application entry point to `WebApplicationFactory<Program>` in the test project, enabling full integration tests without any test-specific configuration.
