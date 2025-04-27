using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using TodoWebApp.Models;
using TodoWebApp.Data;

namespace Todo.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext registration
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Remove existing authentication
            var authDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(JwtBearerHandler));
            if (authDescriptor != null)
            {
                services.Remove(authDescriptor);
            }

            // Create a new service provider for DbContext
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            // Configure test database
            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestingDb");
                options.UseInternalServiceProvider(serviceProvider);
            });

            // Remove and reconfigure authentication
            var descriptors = services.Where(d => d.ServiceType == typeof(IConfigureOptions<JwtBearerOptions>)).ToList();
            foreach (var jwtDescriptor in descriptors)
            {
                services.Remove(jwtDescriptor);
            }

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "TestApi",
                    ValidAudience = "TestApi",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes("your-super-secret-key-with-at-least-32-characters")
                    )
                };
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<TodoDbContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();

            // Clear the database
            db.Todos.RemoveRange(db.Todos);
            db.SaveChanges();
        });
    }

    public string GenerateJwtToken()
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("your-super-secret-key-with-at-least-32-characters");

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.Role, Roles.Admin)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "TestApi",
            Audience = "TestApi",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
} 