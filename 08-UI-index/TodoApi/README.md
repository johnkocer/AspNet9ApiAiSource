# Todo API

A minimal, high-performance RESTful API built with .NET for managing todo items. This project demonstrates the use of minimal APIs, Entity Framework Core with in-memory database, and OpenAPI (Swagger) documentation.

## 🚀 Technologies Used

- .NET (Latest Version)
- Entity Framework Core
- In-Memory Database
- Swagger/OpenAPI
- Minimal API architecture

## 📋 Prerequisites

- .NET SDK (Latest Version)
- An IDE (Visual Studio, VS Code, or JetBrains Rider)

## 🛠️ Setup and Installation

1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run
```

The API will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: https://localhost:5001/swagger

## 📌 API Endpoints

### Todo Operations

| Method | Endpoint      | Description                 | Request Body | Response        |
|--------|--------------|----------------------------|--------------|-----------------|
| GET    | /todos       | Get all todo items         | None         | Array of todos |
| GET    | /todos/{id}  | Get a specific todo item   | None         | Single todo    |
| POST   | /todos       | Create a new todo item     | Todo object  | Created todo   |
| PUT    | /todos/{id}  | Update an existing todo    | Todo object  | No content     |
| DELETE | /todos/{id}  | Delete a todo item         | None         | No content     |

### Data Models

#### Todo
```csharp
public class Todo
{
    public int Id { get; set; }          // Unique identifier
    public string Title { get; set; }     // Required, max length 200
    public bool IsCompleted { get; set; } // Todo completion status
}
```

## 🔧 Configuration

The application uses the following configuration:
- In-memory database for data storage
- Swagger UI enabled in development environment
- HTTPS redirection enabled

## 💡 Features

- RESTful API endpoints
- In-memory data persistence
- OpenAPI documentation
- Input validation
- HTTP status codes for proper error handling
- Automatic model validation
- Dependency injection

## 🔍 API Documentation

The API documentation is available through Swagger UI when running in development mode. Access it at:
```
https://localhost:5001/swagger
```

## 🧪 Testing

You can test the API using:
1. Swagger UI interface
2. The provided `Todo.http` file
3. Any HTTP client (Postman, curl, etc.)

Example curl commands:

```bash
# Get all todos
curl -X GET https://localhost:5001/todos

# Create a new todo
curl -X POST https://localhost:5001/todos \
  -H "Content-Type: application/json" \
  -d '{"title": "New Task", "isCompleted": false}'
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## 📝 License

This project is licensed under the MIT License. 