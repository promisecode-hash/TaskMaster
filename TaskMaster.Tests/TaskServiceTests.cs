using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskMaster.Application.Models;
using TaskMaster.Application.Services;
using TaskMaster.Infrastructure.Data;
using TaskMaster.Infrastructure.Repositories;
using Xunit;

namespace TaskMaster.Tests;

public sealed class TaskServiceTests
{
    private static ILogger<TaskService> CreateNullLogger()
    {
        var loggerFactory = LoggerFactory.Create(builder => { });
        return loggerFactory.CreateLogger<TaskService>();
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedTask()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TaskService(repository, CreateNullLogger());

        var request = new CreateTaskRequest
        {
            Title = "Write architecture doc",
            Description = "Define the clean architecture layers",
            DueDate = DateTime.UtcNow.AddDays(3)
        };

        var result = await service.CreateAsync(request);

        Assert.NotNull(result);
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);
        Assert.Equal(request.DueDate, result.DueDate);
        Assert.Equal(TaskMaster.Domain.Entities.TaskStatus.Backlog, result.Status);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTask()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TaskService(repository, CreateNullLogger());

        var created = await service.CreateAsync(new CreateTaskRequest { Title = "Review architecture" });
        var deleted = await service.DeleteAsync(created.Id);
        var retrieved = await service.GetByIdAsync(created.Id);

        Assert.True(deleted);
        Assert.Null(retrieved);
    }

    [Fact]
    public async Task GetAllAsync_WithStatusFilter_ShouldReturnMatchingTasks()
    {
        var repository = new InMemoryTaskRepository();
        var service = new TaskService(repository, CreateNullLogger());

        await service.CreateAsync(new CreateTaskRequest { Title = "Task 1" });
        var second = await service.CreateAsync(new CreateTaskRequest { Title = "Task 2" });
        await service.UpdateAsync(second.Id, new UpdateTaskRequest { Title = "Task 2", Status = TaskMaster.Domain.Entities.TaskStatus.Completed });

        var result = await service.GetAllAsync(new TaskQuery { Status = TaskMaster.Domain.Entities.TaskStatus.Completed });

        Assert.Single(result.Items);
        Assert.Equal(TaskMaster.Domain.Entities.TaskStatus.Completed, result.Items.First().Status);
    }

    [Fact]
    public async Task EfTaskRepository_ShouldPersistTaskToSqlite()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TaskMasterDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var context = new TaskMasterDbContext(options))
        {
            await context.Database.EnsureCreatedAsync();
            var repository = new EfTaskRepository(context);
            var service = new TaskService(repository, CreateNullLogger());

            var request = new CreateTaskRequest { Title = "Persist task" };
            var created = await service.CreateAsync(request);

            Assert.NotEqual(Guid.Empty, created.Id);
            Assert.Equal(request.Title, created.Title);

            var retrieved = await service.GetByIdAsync(created.Id);
            Assert.NotNull(retrieved);
            Assert.Equal(created.Id, retrieved!.Id);
        }
    }
}
