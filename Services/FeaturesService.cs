using System.Collections.Concurrent;

namespace TestPaliRESTApi.Services;

public class FeaturesService : IFeaturesService
{
    private readonly ConcurrentDictionary<string, bool> _flags = new();

    public bool IsEnabled(string user) => _flags.GetOrAdd(user, true);

    public void SetEnabled(string user, bool enabled) => _flags[user] = enabled;

    public IReadOnlyDictionary<string, bool> GetAll() => _flags;
}
