using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using TodoWebApp.Models;
using TodoWebApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add this method before the builder configuration
static User? AuthenticateUser(string username, string password)
{
    // This is a simple example. In a real application, you should validate against a database
    if (username == "admin" && password == "password")
    {
        return new User 
        { 
            Id = 1,
            Username = username, 
            Role = Roles.Admin,
            Permissions = new List<string> 
            { 
                Permissions.CreateTodo,
                Permissions.UpdateTodo,
                Permissions.DeleteTodo,
                Permissions.ViewTodo,
                Permissions.ManageUsers
            }
        };
    }
    return null;
}

// Add static files service
builder.Services.AddDirectoryBrowser();

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "TodoApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "TodoApi",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-with-at-least-32-characters")
            )
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(Roles.Admin));
    options.AddPolicy("RequireManagerRole", policy => policy.RequireRole(Roles.Manager));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole(Roles.User));
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
  {
    Title = "Todo API",
    Version = "v1"
  });

  c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
  {
    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
    Name = "Authorization",
    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
    Scheme = "Bearer"
  });

  c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1"));
}

// Add static files middleware
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.MapPost("/auth/token", (LoginRequest request) =>
{
    var user = AuthenticateUser(request.Username, request.Password);
    if (user is null) return Results.Unauthorized();

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes(
        app.Configuration["Jwt:Key"] ?? "your-super-secret-key-with-at-least-32-characters"
    );

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString())
        }.Concat(user.Permissions.Select(p => new Claim("Permission", p)))),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = app.Configuration["Jwt:Issuer"] ?? "TodoApi",
        Audience = app.Configuration["Jwt:Audience"] ?? "TodoApi",
        SigningCredentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature
        )
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Results.Ok(new { Token = tokenHandler.WriteToken(token) });
})
.AllowAnonymous();

// In-memory storage for Todo items.
// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    if (!db.Todos.Any())
    {
        db.Todos.AddRange(
            new TodoWebApp.Models.Todo { Id = 1, Name = "Learn Minimal APIs", IsDone = false },
            new TodoWebApp.Models.Todo { Id = 2, Name = "Use In-Memory SQL", IsDone = false }
        );
        db.SaveChanges();
    }
}

// Example endpoint authorize
app.MapGet("/todos", [Authorize] async (TodoDbContext db) =>
    await db.Todos.ToListAsync());

// create new todo item
app.MapPost("/todos", [Authorize] async (TodoWebApp.Models.Todo todo, TodoDbContext db) =>
{
    if (string.IsNullOrEmpty(todo.Name))
        return Results.BadRequest("Name cannot be empty");

    if (todo.Name.Length > 200)
        return Results.BadRequest("Name cannot exceed 200 characters");

    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

// update todo item
app.MapPut("/todos/{id}", [Authorize] async (int id, TodoWebApp.Models.Todo inputTodo, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    if (string.IsNullOrEmpty(inputTodo.Name))
        return Results.BadRequest("Name cannot be empty");

    if (inputTodo.Name.Length > 200)
        return Results.BadRequest("Name cannot exceed 200 characters");

    todo.Name = inputTodo.Name;
    todo.IsDone = inputTodo.IsDone;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// delete todo item
app.MapDelete("/todos/{id}", [Authorize] async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();
    
    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Read a single todo item by ID.
app.MapGet("/todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
});
app.UseAuthentication();
app.UseAuthorization();
app.Run();

// Make the Program class public
public partial class Program { }