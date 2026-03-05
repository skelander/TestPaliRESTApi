using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using TestPaliRESTApi.Data;
using TestPaliRESTApi.Logging;
using TestPaliRESTApi.Models;
using TestPaliRESTApi.Services;

var builder = WebApplication.CreateBuilder(args);

var logStore = new LogStore();
builder.Services.AddSingleton(logStore);
builder.Logging.AddProvider(new InMemoryLoggerProvider(logStore));

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
        };
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
