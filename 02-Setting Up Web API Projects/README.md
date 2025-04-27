# FastApi - ASP.NET 9 Web API Project

## Project Overview
This project is a simple ASP.NET 9 Web API demonstration with a weather forecast endpoint. It utilizes the minimal API approach introduced in .NET 6 and enhanced in ASP.NET 9.

## Technical Stack
- ASP.NET 9
- Minimal API pattern
- OpenAPI (Swagger) for API documentation

## Project Structure
```
FastApi/
├── Program.cs           # Main entry point and API configuration
├── appsettings.json     # Main application settings
├── appsettings.Development.json # Development-specific settings
├── FastApi.csproj       # Project file with dependencies
├── FastApi.http         # HTTP requests file for testing
└── Properties/
    └── launchSettings.json # Launch configuration settings
```

## API Endpoints
- `GET /weatherforecast`: Returns a 5-day weather forecast with random temperatures

## Getting Started

### Prerequisites
- .NET 9 SDK
- An IDE like Visual Studio 2022 or VS Code

### Running the Application
1. Clone the repository
2. Navigate to the project directory
3. Run the following commands:

```bash
dotnet restore
dotnet build
dotnet run
```

4. The API will be available at:
   - HTTPS: https://localhost:7xxx (check launchSettings.json for exact port)
   - HTTP: http://localhost:5xxx (check launchSettings.json for exact port)

### Development Environment
In development mode, OpenAPI (Swagger) is available to help with API testing and documentation.

## Configuration
Configuration is managed through appsettings.json and environment-specific versions (appsettings.Development.json).

## Project Components

### Minimal API Structure
The application uses a minimal API approach, defining routes directly in Program.cs without controllers.

### OpenAPI Integration
OpenAPI is configured for development environments using:
```csharp
builder.Services.AddOpenApi();
```

And mapped in the HTTP pipeline:
```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
```

### Weather Forecast Model
```csharp
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
```

## Further Development
This project serves as a starting point for ASP.NET 9 API development. Consider:

- Adding additional API endpoints
- Implementing a database connection
- Adding authentication and authorization
- Creating a layered architecture (repositories, services)
- Implementing logging and monitoring
- Adding automated tests

## Last Updated
April 23, 2025