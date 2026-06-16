# TaskMaster
TaskMaster is a clean architecture task management solution implemented with .NET.

## Solution structure
- `TaskMaster.Api` — ASP.NET Core Web API entrypoint with REST endpoints for tasks
- `TaskMaster.Application` — application services, DTOs, and use-case abstractions
- `TaskMaster.Domain` — core task entity and business rules
- `TaskMaster.Infrastructure` — SQLite-backed repository implementation for task persistence
- `TaskMaster.Tests` — xUnit tests for application behavior

## Getting started
1. Install the .NET SDK if needed.
2. Open the solution in Visual Studio or VS Code.
3. Run `dotnet restore` from the repository root.
4. Run `dotnet build TaskMaster.sln`.
5. Start the API with `dotnet run --project TaskMaster.Api`.
6. Open Swagger at `https://localhost:5001/swagger` or the URL shown in the console.

## Persistence
- The API uses SQLite via EF Core.
- Database migrations are applied automatically at startup.
- The database file is created automatically at runtime.

## Authentication
- Authenticate with `POST /api/auth/login`.
- Use the default credentials `admin` / `Password123!`.
- Send the returned JWT token in the `Authorization` header as `Bearer <token>`.
- All `/api/tasks` routes require authorization.

## Search and pagination
- Use query parameters on `GET /api/tasks`.
- Supported filters: `searchTerm`, `status`, `dueBefore`, `pageNumber`, `pageSize`.
- Example: `/api/tasks?searchTerm=architecture&pageNumber=1&pageSize=10`.
