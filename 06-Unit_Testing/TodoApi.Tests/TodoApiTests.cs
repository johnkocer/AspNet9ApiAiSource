using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Todo.Tests;

public class TodoApiTests : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly IServiceScope _scope;
    private readonly TodoDbContext _db;

    public TodoApiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        _db = _scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    }

    public async Task InitializeAsync()
    {
        await _db.Database.EnsureCreatedAsync();
        _db.Todos.RemoveRange(_db.Todos.ToList());
        await _db.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        _db.Todos.RemoveRange(_db.Todos.ToList());
        await _db.SaveChangesAsync();
        _scope.Dispose();
    }

    [Fact]
    public async Task CreateTodo_WithValidData_ReturnsCreated()
    {
        // Arrange
        var newTodo = new TodoApi.Todo { Name = "Test Todo" };

        // Act
        var response = await _client.PostAsJsonAsync("/todos", newTodo);
        var createdTodo = await response.Content.ReadFromJsonAsync<TodoApi.Todo>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdTodo);
        Assert.Equal(newTodo.Name, createdTodo.Name);
        Assert.False(createdTodo.IsDone);
    }

    [Fact]
    public async Task CreateTodo_WithEmptyName_ReturnsBadRequest()
    {
        // Arrange
        var newTodo = new TodoApi.Todo { Name = "" };

        // Act
        var response = await _client.PostAsJsonAsync("/todos", newTodo);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_WithNameExceeding200Chars_ReturnsBadRequest()
    {
        // Arrange
        var newTodo = new TodoApi.Todo { Name = new string('x', 201) };

        // Act
        var response = await _client.PostAsJsonAsync("/todos", newTodo);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAllTodos_ReturnsSuccessAndList()
    {
        // Arrange
        await CreateTestTodo("Test Todo 1");
        await CreateTestTodo("Test Todo 2");

        // Act
        var response = await _client.GetAsync("/todos");
        var todos = await response.Content.ReadFromJsonAsync<List<TodoApi.Todo>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(todos);
        Assert.Equal(2, todos.Count);
    }

    [Fact]
    public async Task GetTodoById_WithValidId_ReturnsSuccess()
    {
        // Arrange
        var createdTodo = await CreateTestTodo("Test Todo");

        // Act
        var response = await _client.GetAsync($"/todos/{createdTodo.Id}");
        var todo = await response.Content.ReadFromJsonAsync<TodoApi.Todo>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(todo);
        Assert.Equal(createdTodo.Name, todo.Name);
    }

    [Fact]
    public async Task GetTodoById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/todos/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_WithValidData_ReturnsNoContent()
    {
        // Arrange
        var todo = await CreateTestTodo("Test Todo");
        var updateData = new TodoApi.Todo { Id = todo.Id, Name = todo.Name, IsDone = true };

        // Act
        var response = await _client.PutAsJsonAsync($"/todos/{todo.Id}", updateData);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify the update
        var getResponse = await _client.GetAsync($"/todos/{todo.Id}");
        var updatedTodo = await getResponse.Content.ReadFromJsonAsync<TodoApi.Todo>();
        Assert.True(updatedTodo?.IsDone);
    }

    [Fact]
    public async Task DeleteTodo_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var todo = await CreateTestTodo("Test Todo");

        // Act
        var response = await _client.DeleteAsync($"/todos/{todo.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/todos/{todo.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<TodoApi.Todo> CreateTestTodo(string name)
    {
        var todo = new TodoApi.Todo { Name = name };
        var response = await _client.PostAsJsonAsync("/todos", todo);
        return await response.Content.ReadFromJsonAsync<TodoApi.Todo>();
    }
} 