using TaskMaster.Application.Interfaces;
using TaskMaster.Application.Models;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Services;

public sealed class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;

    public TaskService(ITaskRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<TaskDto>> GetAllAsync(TaskQuery? query = null, CancellationToken cancellationToken = default)
    {
        query ??= new TaskQuery();
        var tasks = await _repository.GetAllAsync(query, cancellationToken);
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
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        return task is null ? null : ToDto(task);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = new TaskItem(request.Title, request.Description, request.DueDate);
        await _repository.AddAsync(task, cancellationToken);
        return ToDto(task);
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var task = await _repository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return null;
        }

        task.Update(request.Title, request.Description, request.DueDate);
        task.SetStatus(request.Status);
        await _repository.UpdateAsync(task, cancellationToken);
        return ToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        await _repository.DeleteAsync(id, cancellationToken);
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
