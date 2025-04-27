using Microsoft.EntityFrameworkCore;
using TodoWebApp.Data;

namespace Todo.Tests;

public static class TestDatabaseHelper
{
    public static TodoDbContext CreateTestDatabase()
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(databaseName: $"TodoDb_{Guid.NewGuid()}")
            .Options;

        var context = new TodoDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static void ClearDatabase(TodoDbContext context)
    {
        context.Todos.RemoveRange(context.Todos);
        context.SaveChanges();
    }
} 