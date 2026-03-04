namespace TestPaliRESTApi.Services;

public class AuthService : IAuthService
{
    private static readonly Dictionary<string, (string Password, string Role)> Users = new()
    {
        { "1",     ("1",     "user")  },
        { "2",     ("2",     "user")  },
        { "3",     ("3",     "user")  },
        { "admin", ("admin", "admin") },
    };

    public string? Authenticate(string username, string password) =>
        Users.TryGetValue(username, out var entry) && entry.Password == password
            ? entry.Role
            : null;
}
