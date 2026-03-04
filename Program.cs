using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TestPaliRESTApi.Data;
using TestPaliRESTApi.Models;
using TestPaliRESTApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("PalindromeDb"));
builder.Services.AddSingleton<IPalindromeService, PalindromeService>();
builder.Services.AddSingleton<IFeaturesService, FeaturesService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("GitHubPages", policy =>
        policy.WithOrigins("https://skelander.github.io")
              .AllowAnyMethod()
              .AllowAnyHeader());
});
builder.Services.AddOpenApi();

var app = builder.Build();

// Seed users
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Users.AddRange(
        new User { Username = "1",     Password = "1",     Role = "user"  },
        new User { Username = "2",     Password = "2",     Role = "user"  },
        new User { Username = "3",     Password = "3",     Role = "user"  },
        new User { Username = "admin", Password = "admin", Role = "admin" }
    );
    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("GitHubPages");
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
