namespace TestPaliRESTApi.Services;

public interface IFeaturesService
{
    bool IsEnabled(string user);
    void SetEnabled(string user, bool enabled);
    IReadOnlyDictionary<string, bool> GetAll();
}
