# Todo API Project Requirements

**Version**: 1.0

**Last Updated**: 2025-04-24

## 1. Overview

Minimal Web API for managing todo items with CRUD operations using ASP.NET Core 9.

## 2. Core Entity

### `Todo` Model

```csharp
public class Todo
{
    public int Id { get; set; }                 // Primary key
    public required string Name { get; set; }   // Task description
    public bool IsDone { get; set; } = false;   // Completion status
}
```

## 3. API Endpoints

| HTTP Method | Endpoint         | Description               |
|-------------|------------------|---------------------------|
| POST        | /api/todos       | Create new todo           |
| GET         | /api/todos       | List all todos            |
| GET         | /api/todos/{id}  | Get todo by ID            |
| PUT         | /api/todos/{id}  | Update completion status  |
| DELETE      | /api/todos/{id}  | Delete todo               |

## 4. Request/Response Examples

### Create Todo

**Request**:

## 5. Validation Rules

- **Name**:
  - Required (400 Bad Request if empty)
  - Max length: 200 characters

## 6. Technical Stack

- ASP.NET Core 9
- Entity Framework Core (In-Memory DB)
- Swagger/OpenAPI documentation

## 7. Quality Standards

- DTOs for input/output models
- Unit tests (80% coverage)
- XML documentation for endpoints

## 8. Out of Scope

- ❌ User authentication
- ❌ Advanced filtering/sorting
- ❌ Notifications
