# Testing Strategy

## Two test files, two responsibilities

### PalindromeServiceTests
Tests the palindrome logic in isolation by instantiating `PalindromeService` directly (`new PalindromeService()`). No HTTP, no DI container, no infrastructure. Fast and focused — if these fail, the problem is in the algorithm.

### PalindromeControllerTests
Tests the full HTTP pipeline using `WebApplicationFactory<Program>`, which boots the real ASP.NET application in memory. Covers routing, serialization, status codes, headers, and response shape. If these fail but service tests pass, the problem is in the HTTP layer.

## Why tests are grouped by input type

Tests are organized by what kind of string is being tested, not by whether the result is true or false. This makes it easy to see what scenarios are covered and to add new cases in the right place.

| Group | What it covers |
|-------|---------------|
| `IsPalindrome_Words` | Single words, including mixed case and near-misses |
| `IsPalindrome_SequentialCharacters` | Purely numeric or alphabetic sequences |
| `IsPalindrome_MixedAlphanumeric` | Strings combining letters and digits |
| `IsPalindrome_Phrases` | Multi-word strings with spaces and punctuation |
| `IsPalindrome_EdgeCases` | Empty string, null, whitespace-only, punctuation-only |

## Why every `[InlineData]` has an explicit `bool expected`

All test methods use `Assert.Equal(expected, _sut.IsPalindrome(input))` with the expected value as a parameter, even in groups where every case is false. This keeps the style consistent across all tests and makes the intent of each case self-documenting without relying on the method name.

## Edge case reasoning

| Input | Result | Reason |
|-------|--------|--------|
| `""` | `true` | Empty string is trivially a palindrome |
| `null` | `true` | Treated as empty string |
| `"   "` | `true` | Strips to empty string — same as above |
| `"!!!"` | `true` | Only whitespace is stripped, so `!!!` stays as-is — and `!!!` reversed is still `!!!` |

These cases are arguably surprising — a string of spaces returning `true` is not obvious. Documenting them in tests makes the behaviour explicit and prevents future changes from accidentally altering it.

## Controller test coverage

| Test | What it verifies |
|------|-----------------|
| `Check_ReturnsCorrectResult` | `isPalindrome` field and status 200 |
| `Check_PhraseWithSpaces_ReturnsPalindrome` | URL decoding — `%20` becomes a space in `input` field |
| `Check_ReturnsJsonContentType` | `Content-Type: application/json` |
| `Check_ReturnsCorrectMessage` | Exact message string for both true and false cases |
| `Check_MissingInput_Returns400` | Query parameter `input` is required; omitting it returns 400 |
| `Check_PostMethod_Returns405` | Only GET is supported |
| `Check_PreservesOriginalCasingInResponse` | `input` field reflects original casing, not lowercased |
