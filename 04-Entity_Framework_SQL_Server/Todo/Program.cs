using Microsoft.OpenApi.Models; // Add this using directive at the top of the file
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "FastApi API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FastApi API V1"));
}

app.UseHttpsRedirection();

// In-memory storage for Todo items.
// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();

    var todos = new List<Todo>
    {
        new Todo { Id = 1, Title = "Learn Minimal APIs", IsCompleted = false },
        new Todo { Id = 2, Title = "Use In-Memory SQL", IsCompleted = false }
    };

    db.Todos.AddRange(todos);
    db.SaveChanges();
}

// Example endpoint
app.MapGet("/todos", async (TodoDbContext db) =>
    await db.Todos.ToListAsync());

// create new todo item
app.MapPost("/todos", async (Todo todo, TodoDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
});

// update todo item
app.MapPut("/todos/{id}", async (int id, Todo todo, TodoDbContext db) =>
{
    var existingTodo = await db.Todos.FindAsync(id);
    if (existingTodo is null) return Results.NotFound();
    existingTodo.Title = todo.Title;
    existingTodo.IsCompleted = todo.IsCompleted;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// delete todo item
app.MapDelete("/todos/{id}", async (int id, TodoDbContext db) =>
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
