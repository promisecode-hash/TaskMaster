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
JWT Bearer Auth + BCrypt password hashing.
- Authenticate with `POST /api/auth/login`.
- Configure credentials outside source control before running or deploying.
- Send the returned JWT token in the `Authorization` header as `Bearer <token>`.
- All `/api/tasks` routes require authorization.

Required auth configuration:
- `Auth__Username`
- `Auth__PasswordHash` — BCrypt hash of the password
- `Auth__Email`
- `Auth__Name`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__ExpiryMinutes`

Example PowerShell session for local development:
```powershell
$env:Auth__Username = "admin"
$env:Auth__PasswordHash = "<bcrypt-password-hash>"
$env:Auth__Email = "admin@example.com"
$env:Auth__Name = "Admin User"
$env:Jwt__Key = "<long-random-secret-key>"
$env:Jwt__Issuer = "TaskMasterApi"
$env:Jwt__Audience = "TaskMasterUsers"
$env:Jwt__ExpiryMinutes = "120"
```

## Search and pagination
- Use query parameters on `GET /api/tasks`.
- Supported filters: `searchTerm`, `status`, `dueBefore`, `pageNumber`, `pageSize`.
- Example: `/api/tasks?searchTerm=architecture&pageNumber=1&pageSize=10`.

## Deployment
- Health check endpoint: `GET /health`.
- The Docker image listens on port `8080`.
- In Docker, the default database path is `/app/data/taskmaster.db`.

Build the image:
```powershell
docker build -t taskmaster-api .
```

Run the container:
```powershell
docker run --rm -p 8080:8080 `
  -e Auth__Username="admin" `
  -e Auth__PasswordHash="<bcrypt-password-hash>" `
  -e Auth__Email="admin@example.com" `
  -e Auth__Name="Admin User" `
  -e Jwt__Key="<long-random-secret-key>" `
  -e Jwt__Issuer="TaskMasterApi" `
  -e Jwt__Audience="TaskMasterUsers" `
  -e Jwt__ExpiryMinutes="120" `
  taskmaster-api
```
