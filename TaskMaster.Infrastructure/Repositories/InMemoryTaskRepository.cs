using TaskMaster.Application.Interfaces;
using TaskMaster.Application.Models;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Infrastructure.Repositories;

public sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly Dictionary<Guid, TaskItem> _store = new();

    public Task<PagedResult<TaskItem>> GetAllAsync(TaskQuery? query = null, CancellationToken cancellationToken = default)
    {
        query ??= new TaskQuery();
        IEnumerable<TaskItem> snapshot = _store.Values;

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var searchTerm = query.SearchTerm.Trim();
            snapshot = snapshot.Where(x => x.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                || (x.Description ?? string.Empty).Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (query.Status is not null)
        {
            snapshot = snapshot.Where(x => x.Status == query.Status.Value);
        }

        if (query.DueBefore is not null)
        {
            snapshot = snapshot.Where(x => x.DueDate.HasValue && x.DueDate <= query.DueBefore.Value);
        }

        var totalItems = snapshot.Count();
        var pageNumber = Math.Max(query.PageNumber, 1);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var pagedItems = snapshot
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<TaskItem>
        {
            Items = pagedItems,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize)
        });
    }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.TryGetValue(id, out var task);
        return Task.FromResult(task);
    }

    public Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        _store[task.Id] = task;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        if (!_store.ContainsKey(task.Id))
        {
            throw new InvalidOperationException($"Task with id '{task.Id}' was not found.");
        }

        _store[task.Id] = task;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _store.Remove(id);
        return Task.CompletedTask;
    }
}
