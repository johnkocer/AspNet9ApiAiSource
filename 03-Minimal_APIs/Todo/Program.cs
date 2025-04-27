using Microsoft.OpenApi.Models; // Add this using directive at the top of the file

var builder = WebApplication.CreateBuilder(args);

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
var todos = new List<Todo>
  {
      new Todo { Id = 1, Title = "Learn Minimal APIs", IsCompleted = false }
  };

// Retrieve all Todo items.
app.MapGet("/todos", () => Results.Ok(todos));

// Retrieve a single Todo item by ID.
app.MapGet("/todos/{id}", (int id) =>
{
  var todo = todos.FirstOrDefault(t => t.Id == id);
  return todo is not null ? Results.Ok(todo) : Results.NotFound();
});

// Create a new Todo item.
app.MapPost("/todos", (Todo todo) =>
{
  var newTodo = new Todo
  {
    Id = todos.Count + 1,
    Title = todo.Title,
    IsCompleted = todo.IsCompleted
  };
  todos.Add(newTodo);
  return Results.Created($"/todos/{newTodo.Id}", newTodo);
});

// Update an existing Todo item.
app.MapPut("/todos/{id}", (int id, Todo updateTodo) =>
{
  var todo = todos.FirstOrDefault(t => t.Id == id);
  if (todo is null) return Results.NotFound();
  var updatedTodo = new Todo
  {
    Id = todo.Id,
    Title = updateTodo.Title,
    IsCompleted = updateTodo.IsCompleted
  };
  todos.Remove(todo);
  todos.Add(updatedTodo);
  return Results.Ok(updatedTodo);
});

// Delete a Todo item.
app.MapDelete("/todos/{id}", (int id) =>
{
  var todo = todos.FirstOrDefault(t => t.Id == id);
  if (todo is null) return Results.NotFound();
  todos.Remove(todo);
  return Results.NoContent();
});

app.Run();

record Todo
{
  public int Id { get; init; }
  public string Title { get; init; }
  public bool IsCompleted { get; init; }
}

