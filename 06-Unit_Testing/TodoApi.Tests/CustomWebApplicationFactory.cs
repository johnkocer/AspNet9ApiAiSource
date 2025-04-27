using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi;

namespace Todo.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Create a new service provider
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<TodoDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestingDb");
                options.UseInternalServiceProvider(serviceProvider);
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
} 