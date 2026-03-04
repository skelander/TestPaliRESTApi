using Microsoft.EntityFrameworkCore;
using TestPaliRESTApi.Models;

namespace TestPaliRESTApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}
