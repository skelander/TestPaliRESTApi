using TestPaliRESTApi.Data;

namespace TestPaliRESTApi.Services;

public class AuthService(AppDbContext db) : IAuthService
{
    public string? Authenticate(string username, string password) =>
        db.Users.FirstOrDefault(u => u.Username == username && u.Password == password)?.Role;
}
