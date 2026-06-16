using TaskMaster.Application.Models;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Interfaces;

public interface ITaskRepository
{
    Task<PagedResult<TaskItem>> GetAllAsync(TaskQuery? query = null, CancellationToken cancellationToken = default);
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

