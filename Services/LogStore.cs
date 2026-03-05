using System.Collections.Concurrent;
using TestPaliRESTApi.Models;

namespace TestPaliRESTApi.Services;

public class LogStore
{
    private readonly ConcurrentQueue<LogEntry> _entries = new();
    private const int MaxEntries = 200;

    public void Add(LogEntry entry)
    {
        _entries.Enqueue(entry);
        while (_entries.Count > MaxEntries)
            _entries.TryDequeue(out _);
    }

    public IReadOnlyList<LogEntry> GetAll() => [.. _entries];
}
