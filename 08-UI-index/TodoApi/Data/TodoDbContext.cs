using Microsoft.EntityFrameworkCore;
using TodoWebApp.Models;

namespace TodoWebApp.Data
{
    public class TodoDbContext : DbContext
    {
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Todo> Todos { get; set; } = null!;
    }
} 