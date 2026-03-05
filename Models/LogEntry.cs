namespace TestPaliRESTApi.Models;

public record LogEntry(DateTimeOffset Timestamp, string Level, string Message);
