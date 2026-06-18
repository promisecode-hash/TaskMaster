FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TaskMaster.sln ./
COPY TaskMaster.Api/TaskMaster.Api.csproj TaskMaster.Api/
COPY TaskMaster.Application/TaskMaster.Application.csproj TaskMaster.Application/
COPY TaskMaster.Domain/TaskMaster.Domain.csproj TaskMaster.Domain/
COPY TaskMaster.Infrastructure/TaskMaster.Infrastructure.csproj TaskMaster.Infrastructure/
COPY TaskMaster.Tests/TaskMaster.Tests.csproj TaskMaster.Tests/

RUN dotnet restore TaskMaster.sln

COPY . .
RUN dotnet publish TaskMaster.Api/TaskMaster.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/taskmaster.db"

RUN mkdir -p /app/data /app/logs

COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskMaster.Api.dll"]
