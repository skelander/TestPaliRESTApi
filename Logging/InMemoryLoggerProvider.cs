using TestPaliRESTApi.Models;
using TestPaliRESTApi.Services;

namespace TestPaliRESTApi.Logging;

public class InMemoryLoggerProvider(LogStore store) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new InMemoryLogger(categoryName, store);
    public void Dispose() { }
}

public class InMemoryLogger(string categoryName, LogStore store) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) =>
        categoryName.StartsWith("TestPaliRESTApi") && logLevel >= LogLevel.Information;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        store.Add(new LogEntry(DateTimeOffset.UtcNow, logLevel.ToString(), formatter(state, exception)));
    }
}
