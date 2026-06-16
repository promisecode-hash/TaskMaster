using Microsoft.Extensions.Logging;
using TaskMaster.Application.Interfaces;
using TaskMaster.Application.Models;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<TaskDto>> GetAllAsync(TaskQuery? query = null, CancellationToken cancellationToken = default)
    {
        query ??= new TaskQuery();
        _logger.LogDebug("Fetching all tasks with query: {@Query}", query);
        var tasks = await _repository.GetAllAsync(query, cancellationToken);
        _logger.LogInformation("Retrieved {Count} tasks from repository", tasks.Items.Count);
        return new PagedResult<TaskDto>
        {
            Items = tasks.Items.Select(ToDto).ToArray(),
            PageNumber = tasks.PageNumber,
            PageSize = tasks.PageSize,
            TotalItems = tasks.TotalItems,
            TotalPages = tasks.TotalPages
        };
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching task by ID: {TaskId}", id);
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Task with ID {TaskId} not found", id);
            return null;
        }
        _logger.LogInformation("Task retrieved with ID: {TaskId}", id);
        return ToDto(task);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new task: {Title}", request.Title);
        var task = new TaskItem(request.Title, request.Description, request.DueDate);
        await _repository.AddAsync(task, cancellationToken);
        _logger.LogInformation("Task created successfully with ID: {TaskId}", task.Id);
        return ToDto(task);
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating task with ID: {TaskId}", id);
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            _logger.LogWarning("Cannot update: Task with ID {TaskId} not found", id);
            return null;
        }

        task.Update(request.Title, request.Description, request.DueDate);
        task.SetStatus(request.Status);
        await _repository.UpdateAsync(task, cancellationToken);
        _logger.LogInformation("Task with ID {TaskId} updated successfully", id);
        return ToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting task with ID: {TaskId}", id);
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            _logger.LogWarning("Cannot delete: Task with ID {TaskId} not found", id);
            return false;
        }

        await _repository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Task with ID {TaskId} deleted successfully", id);
        return true;
    }

    private static TaskDto ToDto(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            CreatedAt = task.CreatedAt,
            DueDate = task.DueDate,
            Status = task.Status
        };
    }
}
