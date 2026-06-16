using TaskMaster.Application.Models;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Application.Interfaces;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetAllAsync(TaskQuery? query = null, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
