namespace TestPaliRESTApi.Services;

public interface IAuthService
{
    string? Authenticate(string username, string password); // returns role or null
}
